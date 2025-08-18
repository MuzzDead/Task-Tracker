using TaskTracker.Client.DTOs.State;
using TaskTracker.Client.DTOs.User;
using TaskTracker.Client.States;

namespace TaskTracker.Client.Services.Interfaces;

public interface ICardModalService
{
    Task<CardModalState> LoadCardDetailsAsync(Guid cardId);
    Task<bool> UpdateCardTitleAsync(Guid cardId, string title, Guid columnId, int rowIndex, Guid userId);
    Task<Guid> CreateCommentAsync(Guid cardId, string content, Guid userId, string username);
    Task<bool> UpdateCommentAsync(Guid commentId, string content, Guid userId);
    Task<bool> DeleteCommentAsync(Guid commentId);
    Task<bool> DeleteCardAsync(Guid cardId);
    Task<StateDto?> GetStateByCardAsync(Guid cardId);
    Task<bool> UpdateCardStateAsync(Guid cardId, bool isCompleted);
    Task<UserDto?> GetAssignedUserAsync(Guid? assigneeId);
    Task<bool> RemoveAssignmentAsync(Guid cardId);
    Task<bool> AssignUserAsync(Guid cardId, Guid userId);
    Task<bool> UpdateCardStateFieldsAsync(Guid cardId, Priority? priority, DateTimeOffset? dateTime, Guid currentUserId);
}