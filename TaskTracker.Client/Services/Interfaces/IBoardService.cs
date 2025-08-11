using Refit;
using TaskTracker.Client.DTOs.Board;
using TaskTracker.Client.DTOs.Pagination;

namespace TaskTracker.Client.Services.Interfaces;

public interface IBoardService
{
    [Get("/board/{id}")]
    Task<BoardDto> GetByIdAsync(Guid id);


    [Get("/board/by-user/{userId}")]
    Task<PagedResult<BoardDto>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 6);


    [Post("/board")]
    Task<Guid> CreateAsync([Body] CreateBoardDto command);


    [Put("/board/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateBoardDto command);


    [Delete("/board/{id}")]
    Task DeleteAsync(Guid id);


    [Put("/board/{id}/archive")]
    Task ArchiveAsync(Guid id);


    [Delete("/board/{boardId}/users/{userId}")]
    Task RemoveUserFromBoardAsync(Guid boardId, Guid userId);

    [Get("/board/search")]
    Task<PagedResult<BoardDto>> SearchAsync(string searchTerm, int page = 1, int pageSize = 6);
}
