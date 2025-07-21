using Refit;
using TaskTracker.Client.DTOs.Board;

namespace TaskTracker.Client.Services.Interfaces;

public interface IBoardService
{
    [Get("/board/{id}")]
    Task<BoardDto> GetByIdAsync(Guid id);


    [Get("/board/by-user/{userId}")]
    Task<List<BoardDto>> GetByUserIdAsync(Guid userId);


    [Post("/board")]
    Task<BoardDto> CreateAsync([Body] CreateBoardDto command);


    [Put("/board/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateBoardDto command);


    [Delete("/board/{id}")]
    Task DeleteAsync(Guid id);


    [Put("/board/{id}/archive")]
    Task ArchiveAsync(Guid id);


    [Delete("/board/{boardId}/users/{userId}")]
    Task RemoveUserFromBoardAsync(Guid boardId, Guid userId);
}
