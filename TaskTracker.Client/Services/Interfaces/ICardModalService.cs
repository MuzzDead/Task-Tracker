using TaskTracker.Client.States;

namespace TaskTracker.Client.Services.Interfaces;

public interface ICardModalService
{
    Task<CardModalState> LoadCardDetailsAsync(Guid cardId);
    Task<bool> UpdateCardTitleAsync(Guid cardId, string title, Guid columnId, int rowIndex, Guid userId);
    Task<bool> CreateCommentAsync(Guid cardId, string content, Guid userId, string username);
    Task<bool> UpdateCommentAsync(Guid commentId, string content, Guid userId);
    Task<bool> DeleteCommentAsync(Guid commentId);
    Task<bool> DeleteCardAsync(Guid cardId);
}