using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.States;

namespace TaskTracker.Client.Services.Interfaces;

public interface IBoardPageService
{
    Task<BoardPageState> LoadBoardAsync(Guid boardId);
    Task<bool> UpdateBoardTitleAsync(Guid boardId, string title, Guid userId);
    Task<bool> CreateColumnAsync(Guid boardId, string title);
    Task<bool> DeleteColumnAsync(Guid columnId);
    Task<bool> CreateCardAsync(Guid columnId, string title, Guid userId);
    Task<List<CardDto>> ReloadCardsForColumnAsync(Guid columnId);
}