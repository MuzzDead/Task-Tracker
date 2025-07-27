using Microsoft.AspNetCore.Components;
using Refit;
using TaskTracker.Client.DTOs.Board;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;
using TaskTracker.Client.DTOs.Comment;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.BoardDetails
{
    public partial class BoardDetails : ComponentBase
    {
        [Parameter] public Guid boardId { get; set; }

        [Inject] private IBoardService BoardService { get; set; } = default!;
        [Inject] private IColumnService ColumnService { get; set; } = default!;
        [Inject] private ICardService CardService { get; set; } = default!;
        [Inject] private ICommentService CommentService { get; set; } = default!;
        [Inject] private IAuthStateService AuthStateService { get; set; } = default!;
        [Inject] NavigationManager Navigation { get; set; } = default!;

        private bool _isLoading = true;
        private bool _isColumnModalVisible;
        private bool _isTitleEditing;
        private bool _isTitleSaving;

        private BoardDto? _board;
        private List<ColumnDto> _columns = new();
        private Dictionary<Guid, List<CardDto>> _cardsByColumn = new();

        private bool _isCardDetailsModalVisible;
        private CardDto? _selectedCard;
        private List<CommentDto> _cardComments = new();

        private bool _isCardTitleEditing;
        private bool _isCardTitleSaving;
        private bool _isCardDescriptionEditing;
        private bool _isCardDescriptionSaving;

        private bool _isCommentsLoading;
        private bool _isCommentSubmitting;

        private bool _isCardDeleting;
        private bool _isColumnDeleting;

        protected override async Task OnInitializedAsync()
        {
            await LoadBoardData();
        }

        private Guid GetCurrentUserId()
        {
            return AuthStateService.CurrentUser?.Id ?? Guid.Empty;
        }

        private async Task OnCardClick(CardDto card)
        {
            _selectedCard = card;
            _isCardDetailsModalVisible = true;
            await LoadCardComments(card.Id);
            StateHasChanged();
        }

        private Task HideCardDetailsModal()
        {
            _isCardDetailsModalVisible = false;
            _selectedCard = null;
            _cardComments.Clear();
            ResetCardEditingStates();
            StateHasChanged();
            return Task.CompletedTask;
        }

        private void ResetCardEditingStates()
        {
            _isCardTitleEditing = false;
            _isCardTitleSaving = false;
            _isCardDescriptionEditing = false;
            _isCardDescriptionSaving = false;
        }

        private Task OnCardTitleEditingChanged(bool isEditing)
        {
            _isCardTitleEditing = isEditing;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task SaveCardTitle(string newTitle)
        {
            if (_selectedCard == null || string.IsNullOrWhiteSpace(newTitle))
                return;

            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            _isCardTitleSaving = true;
            StateHasChanged();

            try
            {
                var updateDto = new UpdateCardDto
                {
                    Id = _selectedCard.Id,
                    Title = newTitle,
                    ColumnId = _selectedCard.ColumnId,
                    RowIndex = _selectedCard.RowIndex,
                    UpdatedBy = currentUserId
                };

                await CardService.UpdateAsync(_selectedCard.Id, updateDto);

                _selectedCard.Title = newTitle;

                if (_cardsByColumn.TryGetValue(_selectedCard.ColumnId, out var columnCards))
                {
                    var cardIndex = columnCards.FindIndex(c => c.Id == _selectedCard.Id);
                    if (cardIndex >= 0)
                    {
                        columnCards[cardIndex].Title = newTitle;
                    }
                }

                _isCardTitleEditing = false;
            }
            catch (ApiException apiEx)
            {
                Console.Error.WriteLine($"[API Error] Failed to update card title: {apiEx.StatusCode}: {apiEx.Content}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error updating card title: {ex.Message}");
            }
            finally
            {
                _isCardTitleSaving = false;
                StateHasChanged();
            }
        }

        private async Task LoadCardComments(Guid cardId)
        {
            _isCommentsLoading = true;
            StateHasChanged();

            try
            {
                _cardComments = (await CommentService.GetByCardIdAsync(cardId)).ToList();
            }
            catch (ApiException apiEx)
            {
                Console.Error.WriteLine($"[API Error] Failed to load comments: {apiEx.StatusCode}: {apiEx.Content}");
                _cardComments.Clear();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading comments: {ex.Message}");
                _cardComments.Clear();
            }
            finally
            {
                _isCommentsLoading = false;
                StateHasChanged();
            }
        }

        private async Task OnCommentSubmit(string commentContent)
        {
            if (_selectedCard == null || string.IsNullOrWhiteSpace(commentContent))
                return;

            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            _isCommentSubmitting = true;
            StateHasChanged();

            try
            {
                var createDto = new CreateCommentDto
                {
                    Text = commentContent,
                    CardId = _selectedCard.Id,
                    UserId = currentUserId
                };

                var newCommentId = await CommentService.CreateAsync(createDto);

                var newComment = new CommentDto
                {
                    Id = newCommentId,
                    Text = commentContent,
                    CardId = _selectedCard.Id,
                    UserId = currentUserId,
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = AuthStateService.CurrentUser?.Username ?? "Unknown"
                };

                _cardComments.Add(newComment);
            }
            catch (ApiException apiEx)
            {
                Console.Error.WriteLine($"[API Error] Failed to create comment: {apiEx.StatusCode}: {apiEx.Content}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error creating comment: {ex.Message}");
            }
            finally
            {
                _isCommentSubmitting = false;
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

            try
            {
                var updateDto = new UpdateCommentDto
                {
                    Id = data.commentId,
                    Text = data.newContent,
                    UpdatedBy = currentUserId
                };

                await CommentService.UpdateAsync(data.commentId, updateDto);

                var comment = _cardComments.FirstOrDefault(c => c.Id == data.commentId);
                if (comment != null)
                {
                    comment.Text = data.newContent;
                    StateHasChanged();
                }
            }
            catch (ApiException apiEx)
            {
                Console.Error.WriteLine($"[API Error] Failed to update comment: {apiEx.StatusCode}: {apiEx.Content}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error updating comment: {ex.Message}");
            }
        }

        private async Task OnCommentDelete(Guid commentId)
        {
            try
            {
                await CommentService.DeleteAsync(commentId);
                _cardComments.RemoveAll(c => c.Id == commentId);
                StateHasChanged();
            }
            catch (ApiException apiEx)
            {
                Console.Error.WriteLine($"[API Error] Failed to delete comment: {apiEx.StatusCode}: {apiEx.Content}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error deleting comment: {ex.Message}");
            }
        }

        private async Task OnCardDelete(Guid cardId)
        {
            if (_isCardDeleting) return;

            _isCardDeleting = true;
            StateHasChanged();

            try
            {
                await CardService.DeleteAsync(cardId);

                foreach (var columnCards in _cardsByColumn.Values)
                {
                    columnCards.RemoveAll(c => c.Id == cardId);
                }

                if (_selectedCard?.Id == cardId)
                {
                    await HideCardDetailsModal();
                }
            }
            catch (ApiException apiEx)
            {
                Console.Error.WriteLine($"[API Error] Failed to delete card: {apiEx.StatusCode}: {apiEx.Content}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error deleting card: {ex.Message}");
            }
            finally
            {
                _isCardDeleting = false;
                StateHasChanged();
            }
        }

        private async Task OnColumnDelete(ColumnDto column)
        {
            if (_isColumnDeleting) return;

            _isColumnDeleting = true;
            StateHasChanged();

            try
            {
                await ColumnService.DeleteAsync(column.Id);

                _columns.RemoveAll(c => c.Id == column.Id);
                _cardsByColumn.Remove(column.Id);
            }
            catch (ApiException apiEx)
            {
                Console.Error.WriteLine($"[API Error] Failed to delete column: {apiEx.StatusCode}: {apiEx.Content}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error deleting column: {ex.Message}");
            }
            finally
            {
                _isColumnDeleting = false;
                StateHasChanged();
            }
        }

        private async Task OnColumnEdit(ColumnDto column)
        {
            Console.WriteLine($"Edit column: {column.Title}");
            await Task.CompletedTask;
        }

        private async Task LoadBoardData()
        {
            _isLoading = true;

            try
            {
                _board = await BoardService.GetByIdAsync(boardId);
                _columns = (await ColumnService.GetByBoardIdAsync(boardId)).ToList();

                var cardTasks = _columns.Select(async column => new
                {
                    ColumnId = column.Id,
                    Cards = (await CardService.GetByColumnIdAsync(column.Id)).ToList()
                });

                _cardsByColumn = (await Task.WhenAll(cardTasks))
                    .ToDictionary(r => r.ColumnId, r => r.Cards);
            }
            catch (ApiException apiEx)
            {
                Console.Error.WriteLine($"[API Error] {apiEx.StatusCode}: {apiEx.Content}");
                SetErrorState();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                SetErrorState();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void SetErrorState()
        {
            _board = new BoardDto { Id = boardId, Title = "Error loading board" };
            _columns.Clear();
            _cardsByColumn.Clear();
        }

        private void ShowColumnModal()
        {
            _isColumnModalVisible = true;
        }

        private Task HideColumnModal()
        {
            _isColumnModalVisible = false;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task HandleColumnCreated(CreateColumnDto dto)
        {
            var nextIndex = _columns.Any() ? _columns.Max(c => c.ColumnIndex) + 1 : 0;

            var newId = await ColumnService.CreateAsync(new CreateColumnDto
            {
                Title = dto.Title,
                BoardId = dto.BoardId,
                ColumnIndex = nextIndex
            });

            var newColumn = new ColumnDto
            {
                Id = newId,
                Title = dto.Title,
                BoardId = dto.BoardId,
                ColumnIndex = nextIndex,
                CreatedAt = DateTimeOffset.Now
            };

            _columns.Add(newColumn);
            _cardsByColumn[newId] = new();
            StateHasChanged();
        }

        private async Task OnAddCard((string title, Guid columnId) data)
        {
            if (await TryCreateCard(data.title, data.columnId))
            {
                await ReloadCardsForColumn(data.columnId);
            }
        }

        private async Task<bool> TryCreateCard(string title, Guid columnId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return false;
            }

            try
            {
                var command = new CreateCardDto
                {
                    Title = title,
                    ColumnId = columnId,
                    CreatedBy = currentUserId
                };
                var cardId = await CardService.CreateAsync(command);
                return cardId != Guid.Empty;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error creating card: {ex.Message}");
                return false;
            }
        }

        private async Task ReloadCardsForColumn(Guid columnId)
        {
            try
            {
                var cards = await CardService.GetByColumnIdAsync(columnId);
                _cardsByColumn[columnId] = cards.ToList();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reloading cards: {ex.Message}");
            }
        }

        private Task OnTitleEditingChanged(bool isEditing)
        {
            _isTitleEditing = isEditing;
            StateHasChanged();
            return Task.CompletedTask;
        }

        private async Task SaveBoardTitle(string newTitle)
        {
            if (_board == null || string.IsNullOrWhiteSpace(newTitle))
                return;

            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
            {
                Console.Error.WriteLine("User not authenticated");
                return;
            }

            _isTitleSaving = true;
            StateHasChanged();

            try
            {
                var updateDto = new UpdateBoardDto
                {
                    Id = _board.Id,
                    Title = newTitle,
                    Description = _board.Description,
                    UpdatedBy = currentUserId
                };
                await BoardService.UpdateAsync(_board.Id, updateDto);

                _board.Title = newTitle;
                _isTitleEditing = false;
            }
            catch (ApiException apiEx)
            {
                Console.Error.WriteLine($"[API Error] Failed to update board title: {apiEx.StatusCode}: {apiEx.Content}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error updating board title: {ex.Message}");
            }
            finally
            {
                _isTitleSaving = false;
                StateHasChanged();
            }
        }
    }
}