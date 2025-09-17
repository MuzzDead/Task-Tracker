using AntDesign;
using Microsoft.AspNetCore.Components.Forms;
using TaskTracker.Client.Components.Comment;
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
    private readonly ICommentService _commentService;

    public CardManager(
        IBoardPageService boardPageService,
        ICardModalService cardModalService,
        Func<Guid> getCurrentUserId,
        IAuthStateService authStateService,
        Func<BoardPageState> getBoardState,
        Func<CardModalState> getCardModalState,
        Action<BoardPageState> setBoardState,
        Action<CardModalState> setCardModalState,
        ICommentService commentService)
    {
        _boardPageService = boardPageService;
        _cardModalService = cardModalService;
        _getCurrentUserId = getCurrentUserId;
        _authStateService = authStateService;
        _getBoardState = getBoardState;
        _getCardModalState = getCardModalState;
        _setBoardState = setBoardState;
        _setCardModalState = setCardModalState;
        _commentService = commentService;
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
        var initialState = CardModalState.WithCard(card);
        _setCardModalState(initialState);

        try
        {
            var cardDetails = await _cardModalService.LoadCardDetailsAsync(card.Id);

            cardDetails.SelectedCard = card;
            cardDetails.IsVisible = true;

            if (cardDetails.AssignedUser != null)
            {
                var currentUserId = _getCurrentUserId();
                cardDetails.IsCurrentUserAssigned = cardDetails.State?.AssigneeId == currentUserId;
            }

            _setCardModalState(cardDetails);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading card details: {ex.Message}");

            initialState.IsCommentsLoading = false;
            initialState.IsAssigneeLoading = false;
            initialState.SetComments(new List<CommentDto>());
            _setCardModalState(initialState);
        }
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

    public async Task OnCardMoveAsync(MoveCardDto card)
    {
        var cardModalState = _getCardModalState();
        var boardState = _getBoardState();

        try
        {
            var success = await _cardModalService.MoveCardAsync(card);
            if (success)
            {
                boardState.MoveCardToColumn(card.CardId, card.ColumnId);
                _setBoardState(boardState);

                if (cardModalState.SelectedCard?.Id == card.CardId)
                {
                    cardModalState.SelectedCard.ColumnId = card.ColumnId;
                    var updatedCard = boardState.CardsByColumn[card.ColumnId]
                        .FirstOrDefault(c => c.Id == card.CardId);
                    if (updatedCard != null)
                    {
                        cardModalState.SelectedCard.RowIndex = updatedCard.RowIndex;
                    }
                }
                _setCardModalState(cardModalState);
            }
            else
            {
                Console.Error.WriteLine("Failed to move card on backend");
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error moving card: {ex.Message}");
        }
    }


    public async Task OnCommentSubmitAsync(CommentSubmissionData submissionData)
    {
        var cardModalState = _getCardModalState();
        if (cardModalState.SelectedCard == null ||
            (string.IsNullOrWhiteSpace(submissionData.Text) && !submissionData.Files.Any()))
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

            var browserFiles = submissionData.Files
                .Where(f => f != null)
                .Select(f => f!)
                .ToList();

            var commentId = await _cardModalService.CreateCommentAsync(
                cardModalState.SelectedCard.Id,
                submissionData.Text,
                currentUserId,
                username,
                browserFiles);
            
            var createdComment = await _commentService.GetByCardIdAsync(cardModalState.SelectedCard.Id);
            var newComment = createdComment.FirstOrDefault(c => c.Id == commentId);

            if (newComment != null)
            {
                var currentCardModalState = _getCardModalState();
                currentCardModalState.AddComment(newComment);
                _setCardModalState(currentCardModalState);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error submitting comment: {ex.Message}");
        }
        finally
        {
            var finalCardModalState = _getCardModalState();
            finalCardModalState.IsCommentSubmitting = false;
            _setCardModalState(finalCardModalState);
        }
    }

    public async Task OnCommentSubmitAsync(string commentContent)
    {
        var submissionData = new CommentSubmissionData
        {
            Text = commentContent,
            Files = new List<IBrowserFile>()
        };

        await OnCommentSubmitAsync(submissionData);

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