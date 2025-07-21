using Refit;
using TaskTracker.Client.DTOs.Board;

namespace TaskTracker.Client.Services.Interfaces;

public interface IBoardService
{
    [Get("/api/board/{id}")]
    Task<BoardDto> GetByIdAsync(Guid id);


    [Get("/api/board/by-user/{userId}")]
    Task<List<BoardDto>> GetByUserIdAsync(Guid userId);


    [Post("/api/board")]
    Task<BoardDto> CreateAsync([Body] CreateBoardDto command);


    [Put("/api/board/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateBoardDto command);


    [Delete("/api/board/{id}")]
    Task DeleteAsync(Guid id);

    
    [Put("/api/board/{id}/archive")]
    Task ArchiveAsync(Guid id);

    
    [Delete("/api/board/{boardId}/users/{userId}")]
    Task RemoveUserFromBoardAsync(Guid boardId, Guid userId);
}
