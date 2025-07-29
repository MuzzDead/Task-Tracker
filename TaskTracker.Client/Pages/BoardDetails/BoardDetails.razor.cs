using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;
using TaskTracker.Client.DTOs.Comment;
using TaskTracker.Client.Services.Interfaces;
using TaskTracker.Client.States;

namespace TaskTracker.Client.Pages.BoardDetails
{
    public partial class BoardDetails : ComponentBase
    {
        [Parameter] public Guid boardId { get; set; }

        [Inject] private IBoardPageService BoardPageService { get; set; } = default!;
        [Inject] private ICardModalService CardModalService { get; set; } = default!;
        [Inject] private IAuthStateService AuthStateService { get; set; } = default!;

        private BoardPageState _boardState = BoardPageState.Loading();
        private CardModalState _cardModalState = CardModalState.Hidden();

        protected override async Task OnInitializedAsync()
        {
            await LoadBoardData();
        }


        private async Task LoadBoardData()
        {
            _boardState = BoardPageState.Loading();
            StateHasChanged();

            _boardState = await BoardPageService.LoadBoardAsync(boardId);
            StateHasChanged();
        }

        private async Task SaveBoardTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                return;

            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            _boardState.IsTitleSaving = true;
            StateHasChanged();

            try
            {
                var success = await BoardPageService.UpdateBoardTitleAsync(boardId, newTitle, currentUserId);
                if (success)
                {
                    _boardState.UpdateBoardTitle(newTitle);
                    _boardState.IsTitleEditing = false;
                }
            }
            finally
            {
                _boardState.IsTitleSaving = false;
                StateHasChanged();
            }
        }

        private Task OnTitleEditingChanged(bool isEditing)
        {
            _boardState.IsTitleEditing = isEditing;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private void ShowColumnModal()
        {
            _boardState.IsColumnModalVisible = true;
            StateHasChanged();
        }

        private Task HideColumnModal()
        {
            _boardState.IsColumnModalVisible = false;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task HandleColumnCreated(CreateColumnDto dto)
        {
            var success = await BoardPageService.CreateColumnAsync(dto.BoardId, dto.Title);
            if (success)
            {
                await LoadBoardData();
            }

            _boardState.IsColumnModalVisible = false;
            StateHasChanged();
        }

        private async Task OnColumnDelete(ColumnDto column)
        {
            if (_boardState.IsColumnDeleting) return;

            _boardState.IsColumnDeleting = true;
            StateHasChanged();

            try
            {
                var success = await BoardPageService.DeleteColumnAsync(column.Id);
                if (success)
                {
                    _boardState.RemoveColumn(column.Id);
                }
            }
            finally
            {
                _boardState.IsColumnDeleting = false;
                StateHasChanged();
            }
        }

        private async Task OnColumnEdit(ColumnDto column)
        {
            Console.WriteLine($"Edit column: {column.Title}");
            await Task.CompletedTask;
        }

        private async Task OnAddCard((string title, Guid columnId) data)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            var success = await BoardPageService.CreateCardAsync(data.columnId, data.title, currentUserId);
            if (success)
            {
                var updatedCards = await BoardPageService.ReloadCardsForColumnAsync(data.columnId);
                _boardState.UpdateCardsForColumn(data.columnId, updatedCards);
                StateHasChanged();
            }
        }

        private async Task OnCardClick(CardDto card)
        {
            _cardModalState = CardModalState.WithCard(card);
            StateHasChanged();

            var cardDetails = await CardModalService.LoadCardDetailsAsync(card.Id);
            _cardModalState.SetComments(cardDetails.Comments);
            StateHasChanged();
        }

        private Task HideCardDetailsModal()
        {
            _cardModalState = CardModalState.Hidden();
            StateHasChanged();
            return Task.CompletedTask;
        }

        private Task OnCardTitleEditingChanged(bool isEditing)
        {
            _cardModalState.IsTitleEditing = isEditing;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task SaveCardTitle(string newTitle)
        {
            if (_cardModalState.SelectedCard == null || string.IsNullOrWhiteSpace(newTitle))
                return;

            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            _cardModalState.IsTitleSaving = true;
            StateHasChanged();

            try
            {
                var card = _cardModalState.SelectedCard;
                var success = await CardModalService.UpdateCardTitleAsync(
                    card.Id, newTitle, card.ColumnId, card.RowIndex, currentUserId);

                if (success)
                {
                    _cardModalState.UpdateCardTitle(newTitle);

                    if (_boardState.CardsByColumn.TryGetValue(card.ColumnId, out var columnCards))
                    {
                        var cardIndex = columnCards.FindIndex(c => c.Id == card.Id);
                        if (cardIndex >= 0)
                        {
                            columnCards[cardIndex].Title = newTitle;
                        }
                    }

                    _cardModalState.IsTitleEditing = false;
                }
            }
            finally
            {
                _cardModalState.IsTitleSaving = false;
                StateHasChanged();
            }
        }

        private async Task OnCardDelete(Guid cardId)
        {
            if (_cardModalState.IsCardDeleting) return;

            _cardModalState.IsCardDeleting = true;
            StateHasChanged();

            try
            {
                var success = await CardModalService.DeleteCardAsync(cardId);
                if (success)
                {
                    _boardState.RemoveCardFromColumn(cardId);

                    if (_cardModalState.SelectedCard?.Id == cardId)
                    {
                        await HideCardDetailsModal();
                    }
                }
            }
            finally
            {
                _cardModalState.IsCardDeleting = false;
                StateHasChanged();
            }
        }

        private async Task OnCommentSubmit(string commentContent)
        {
            if (_cardModalState.SelectedCard == null || string.IsNullOrWhiteSpace(commentContent))
                return;

            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            _cardModalState.IsCommentSubmitting = true;
            StateHasChanged();

            try
            {
                var username = AuthStateService.CurrentUser?.Username ?? "Unknown";
                var success = await CardModalService.CreateCommentAsync(
                    _cardModalState.SelectedCard.Id, commentContent, currentUserId, username);

                if (success)
                {
                    var newComment = new CommentDto
                    {
                        Id = Guid.NewGuid(),
                        Text = commentContent,
                        CardId = _cardModalState.SelectedCard.Id,
                        UserId = currentUserId,
                        CreatedAt = DateTimeOffset.Now,
                        CreatedBy = username
                    };

                    _cardModalState.AddComment(newComment);
                }
            }
            finally
            {
                _cardModalState.IsCommentSubmitting = false;
                StateHasChanged();
            }
        }

        private async Task OnCommentEdit((Guid commentId, string newContent) data)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            var success = await CardModalService.UpdateCommentAsync(data.commentId, data.newContent, currentUserId);
            if (success)
            {
                _cardModalState.UpdateComment(data.commentId, data.newContent);
                StateHasChanged();
            }
        }

        private async Task OnCommentDelete(Guid commentId)
        {
            var success = await CardModalService.DeleteCommentAsync(commentId);
            if (success)
            {
                _cardModalState.RemoveComment(commentId);
                StateHasChanged();
            }
        }

        private Guid GetCurrentUserId()
        {
            return AuthStateService.CurrentUser?.Id ?? Guid.Empty;
        }
    }
}