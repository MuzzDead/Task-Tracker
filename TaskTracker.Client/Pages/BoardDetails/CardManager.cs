using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Comment;
using TaskTracker.Client.Services.Interfaces;
using TaskTracker.Client.States;

namespace TaskTracker.Client.Pages.BoardDetails;

public class CardManager
{
    private readonly IBoardPageService _boardPageService;
    private readonly ICardModalService _cardModalService;
    private readonly Func<Guid> _getCurrentUserId;
    private readonly IAuthStateService _authStateService;
    private readonly Func<BoardPageState> _getBoardState;
    private readonly Func<CardModalState> _getCardModalState;
    private readonly Action<BoardPageState> _setBoardState;
    private readonly Action<CardModalState> _setCardModalState;

    public CardManager(
        IBoardPageService boardPageService,
        ICardModalService cardModalService,
        Func<Guid> getCurrentUserId,
        IAuthStateService authStateService,
        Func<BoardPageState> getBoardState,
        Func<CardModalState> getCardModalState,
        Action<BoardPageState> setBoardState,
        Action<CardModalState> setCardModalState)
    {
        _boardPageService = boardPageService;
        _cardModalService = cardModalService;
        _getCurrentUserId = getCurrentUserId;
        _authStateService = authStateService;
        _getBoardState = getBoardState;
        _getCardModalState = getCardModalState;
        _setBoardState = setBoardState;
        _setCardModalState = setCardModalState;
    }

    public async Task OnAddCardAsync((string title, Guid columnId) data)
    {
        var currentUserId = _getCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            Console.Error.WriteLine("User not authenticated");
            return;
        }

        var success = await _boardPageService.CreateCardAsync(data.columnId, data.title, currentUserId);
        if (success)
        {
            var updatedCards = await _boardPageService.ReloadCardsForColumnAsync(data.columnId);
            var boardState = _getBoardState();
            boardState.UpdateCardsForColumn(data.columnId, updatedCards);
            _setBoardState(boardState);
        }
    }

    public async Task OnCardClickAsync(CardDto card)
    {
        var cardModalState = CardModalState.WithCard(card);
        _setCardModalState(cardModalState);

        var cardDetails = await _cardModalService.LoadCardDetailsAsync(card.Id);
        cardModalState.SetComments(cardDetails.Comments);
        _setCardModalState(cardModalState);
    }

    public Task HideCardDetailsModal()
    {
        _setCardModalState(CardModalState.Hidden());
        return Task.CompletedTask;
    }

    public Task OnCardTitleEditingChanged(bool isEditing)
    {
        var cardModalState = _getCardModalState();
        cardModalState.IsTitleEditing = isEditing;
        _setCardModalState(cardModalState);
        return Task.CompletedTask;
    }

    public async Task SaveCardTitleAsync(string newTitle)
    {
        var cardModalState = _getCardModalState();
        if (cardModalState.SelectedCard == null || string.IsNullOrWhiteSpace(newTitle))
            return;

        var currentUserId = _getCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            Console.Error.WriteLine("User not authenticated");
            return;
        }

        cardModalState.IsTitleSaving = true;
        _setCardModalState(cardModalState);

        try
        {
            var card = cardModalState.SelectedCard;
            var success = await _cardModalService.UpdateCardTitleAsync(
                card.Id, newTitle, card.ColumnId, card.RowIndex, currentUserId);

            if (success)
            {
                cardModalState.UpdateCardTitle(newTitle);

                var boardState = _getBoardState();
                if (boardState.CardsByColumn.TryGetValue(card.ColumnId, out var columnCards))
                {
                    var cardIndex = columnCards.FindIndex(c => c.Id == card.Id);
                    if (cardIndex >= 0)
                    {
                        columnCards[cardIndex].Title = newTitle;
                    }
                }

                cardModalState.IsTitleEditing = false;
                _setBoardState(boardState);
            }
        }
        finally
        {
            cardModalState.IsTitleSaving = false;
            _setCardModalState(cardModalState);
        }
    }

    public async Task OnCardDeleteAsync(Guid cardId)
    {
        var cardModalState = _getCardModalState();
        if (cardModalState.IsCardDeleting) return;

        cardModalState.IsCardDeleting = true;
        _setCardModalState(cardModalState);

        try
        {
            var success = await _cardModalService.DeleteCardAsync(cardId);
            if (success)
            {
                var boardState = _getBoardState();
                boardState.RemoveCardFromColumn(cardId);
                _setBoardState(boardState);

                if (cardModalState.SelectedCard?.Id == cardId)
                {
                    await HideCardDetailsModal();
                }
            }
        }
        finally
        {
            cardModalState.IsCardDeleting = false;
            _setCardModalState(cardModalState);
        }
    }

    public async Task OnCommentSubmitAsync(string commentContent)
    {
        var cardModalState = _getCardModalState();
        if (cardModalState.SelectedCard == null || string.IsNullOrWhiteSpace(commentContent))
            return;

        var currentUserId = _getCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            Console.Error.WriteLine("User not authenticated");
            return;
        }

        cardModalState.IsCommentSubmitting = true;
        _setCardModalState(cardModalState);

        try
        {
            var username = _authStateService.CurrentUser?.Username ?? "Unknown";
            var commentId = await _cardModalService.CreateCommentAsync(
                cardModalState.SelectedCard.Id, commentContent, currentUserId, username);

            if (commentId != Guid.Empty)
            {
                var newComment = new CommentDto
                {
                    Id = commentId,
                    Text = commentContent,
                    CardId = cardModalState.SelectedCard.Id,
                    UserId = currentUserId,
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = username
                };

                cardModalState.AddComment(newComment);
            }
        }
        finally
        {
            cardModalState.IsCommentSubmitting = false;
            _setCardModalState(cardModalState);
        }
    }

    public async Task OnCommentEditAsync((Guid commentId, string newContent) data)
    {
        var currentUserId = _getCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            Console.Error.WriteLine("User not authenticated");
            return;
        }

        var success = await _cardModalService.UpdateCommentAsync(data.commentId, data.newContent, currentUserId);
        if (success)
        {
            var cardModalState = _getCardModalState();
            cardModalState.UpdateComment(data.commentId, data.newContent);
            _setCardModalState(cardModalState);
        }
    }

    public async Task OnCommentDeleteAsync(Guid commentId)
    {
        var success = await _cardModalService.DeleteCommentAsync(commentId);
        if (success)
        {
            var cardModalState = _getCardModalState();
            cardModalState.RemoveComment(commentId);
            _setCardModalState(cardModalState);
        }
    }
}