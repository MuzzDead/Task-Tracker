using Refit;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Comment;
using TaskTracker.Client.DTOs.State;
using TaskTracker.Client.DTOs.User;
using TaskTracker.Client.Services.Interfaces;
using TaskTracker.Client.States;

namespace TaskTracker.Client.Services;

public class CardModalService : ICardModalService
{
    private readonly ICardService _cardService;
    private readonly ICommentService _commentService;
    private readonly IStateService _stateService;
    private readonly IUserService _userService;

    public CardModalService(ICardService cardService, ICommentService commentService, IStateService stateService, IUserService userService)
    {
        _cardService = cardService;
        _commentService = commentService;
        _stateService = stateService;
        _userService = userService;
    }

    public async Task<CardModalState> LoadCardDetailsAsync(Guid cardId)
    {
        var state = new CardModalState
        {
            IsCommentsLoading = true,
            IsAssigneeLoading = true
        };

        try
        {
            var cardStateTask = _stateService.GetByCardIdAsync(cardId);
            var commentsTask = _commentService.GetByCardIdAsync(cardId);

            await Task.WhenAll(cardStateTask, commentsTask);

            var cardState = await cardStateTask;
            var comments = (await commentsTask).ToList();

            state.SetCardStates(cardState);
            state.SetComments(comments);

            if (cardState?.AssigneeId != null)
            {
                var assignedUser = await GetAssignedUserAsync(cardState.AssigneeId.Value);
                if (assignedUser != null)
                {
                    state.SetAssignedUser(assignedUser);
                }
            }
            else
            {
                state.IsAssigneeLoading = false;
            }

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
            state.IsAssigneeLoading = false;
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

    public async Task<bool> UpdateCardStateAsync(Guid cardId, bool isCompleted)
    {
        try
        {
            var updateDto = new UpdateStateDto
            {
                CardId = cardId,
                IsCompleted = isCompleted
            };

            await _stateService.UpdateAsync(cardId, updateDto);
            return true;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to update card state: {apiEx.StatusCode}: {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating card state: {ex.Message}");
            return false;
        }
    }

    public async Task<StateDto?> GetStateByCardAsync(Guid cardId)
    {
        try
        {
            return await _stateService.GetByCardIdAsync(cardId);
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to get card state: {apiEx.StatusCode}: {apiEx.Content}");
            throw;
        }
    }

    public async Task<UserDto?> GetAssignedUserAsync(Guid? assigneeId)
    {
        if (!assigneeId.HasValue) return null;

        try
        {
            return await _userService.GetByIdAsync(assigneeId.Value);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading assigned user: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> RemoveAssignmentAsync(Guid cardId)
    {
        try
        {
            var updateDto = new UpdateStateDto
            {
                CardId = cardId,
                AssigneeId = Guid.Empty
            };

            await _stateService.UpdateAsync(cardId, updateDto);
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error removing assignment: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> AssignUserAsync(Guid cardId, Guid userId)
    {
        try
        {
            var updateDto = new UpdateStateDto
            {
                CardId = cardId,
                AssigneeId = userId
            };

            await _stateService.UpdateAsync(cardId, updateDto);
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error assigning user: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateCardStateFieldsAsync(Guid cardId, Priority? priority, DateTimeOffset? deadline, Guid currentUserId)
    {
        try
        {
            var updateDto = new UpdateStateDto
            {
                CardId = cardId,
                Priority = priority,
                Deadline = deadline,
                UpdatedBy = currentUserId
            };

            await _stateService.UpdateAsync(cardId, updateDto);
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error updating card state fields: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> MoveCardAsync(MoveCardDto card)
    {
        try
        {
            await _cardService.MoveAsync(card.CardId, card);
            return true;
        }
        catch (ApiException apiEx)
        {
            Console.Error.WriteLine($"[API Error] Failed to move card: {apiEx.StatusCode}: {apiEx.Content}");
            throw;
        }
    }
}