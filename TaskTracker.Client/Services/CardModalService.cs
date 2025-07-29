using Refit;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Comment;
using TaskTracker.Client.Services.Interfaces;
using TaskTracker.Client.States;

namespace TaskTracker.Client.Services;

public class CardModalService : ICardModalService
{
    private readonly ICardService _cardService;
    private readonly ICommentService _commentService;

    public CardModalService(ICardService cardService, ICommentService commentService)
    {
        _cardService = cardService;
        _commentService = commentService;
    }

    public async Task<CardModalState> LoadCardDetailsAsync(Guid cardId)
    {
        var state = new CardModalState
        {
            IsCommentsLoading = true
        };

        try
        {
            var comments = (await _commentService.GetByCardIdAsync(cardId)).ToList();
            state.SetComments(comments);
            return state;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to load comments: {apiEx.StatusCode}: {apiEx.Content}");
            state.SetComments(new List<CommentDto>());
            return state;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading comments: {ex.Message}");
            state.SetComments(new List<CommentDto>());
            return state;
        }
    }

    public async Task<bool> UpdateCardTitleAsync(Guid cardId, string title, Guid columnId, int rowIndex, Guid userId)
    {
        try
        {
            var updateDto = new UpdateCardDto
            {
                Id = cardId,
                Title = title,
                ColumnId = columnId,
                RowIndex = rowIndex,
                UpdatedBy = userId
            };

            await _cardService.UpdateAsync(cardId, updateDto);
            return true;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to update card title: {apiEx.StatusCode}: {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating card title: {ex.Message}");
            return false;
        }
    }

    public async Task<Guid> CreateCommentAsync(Guid cardId, string content, Guid userId, string username)
    {
        try
        {
            var createDto = new CreateCommentDto
            {
                Text = content,
                CardId = cardId,
                UserId = userId,
                CreatedBy = username
            };

            var commentId = await _commentService.CreateAsync(createDto);
            return commentId;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to create comment: {apiEx.StatusCode}: {apiEx.Content}");
            return Guid.Empty;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating comment: {ex.Message}");
            return Guid.Empty;
        }
    }

    public async Task<bool> UpdateCommentAsync(Guid commentId, string content, Guid userId)
    {
        try
        {
            var updateDto = new UpdateCommentDto
            {
                Id = commentId,
                Text = content,
                UpdatedBy = userId
            };

            await _commentService.UpdateAsync(commentId, updateDto);
            return true;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to update comment: {apiEx.StatusCode}: {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating comment: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteCommentAsync(Guid commentId)
    {
        try
        {
            await _commentService.DeleteAsync(commentId);
            return true;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to delete comment: {apiEx.StatusCode}: {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting comment: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteCardAsync(Guid cardId)
    {
        try
        {
            await _cardService.DeleteAsync(cardId);
            return true;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to delete card: {apiEx.StatusCode}: {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting card: {ex.Message}");
            return false;
        }
    }
}