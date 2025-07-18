using TaskTracker.Client.DTOs.Board;
using TaskTracker.Client.DTOs.Card;
using TaskTracker.Client.DTOs.Column;

namespace TaskTracker.Client.Services.Interfaces;

public interface IBoardService
{
    Task<List<BoardDto>> GetBoardsByUserAsync(Guid userId);
    Task<BoardDto?> GetBoardByIdAsync(Guid boardId);
    Task<List<ColumnDto>> GetColumnsAsync(Guid boardId);
    Task<List<CardDto>> GetCardsAsync(Guid columnId);
}
