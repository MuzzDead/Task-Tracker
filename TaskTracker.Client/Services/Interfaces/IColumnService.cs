using Refit;
using TaskTracker.Client.DTOs.Column;

namespace TaskTracker.Client.Services.Interfaces;

public interface IColumnService
{
    [Get("/column/by-id/{id}")]
    Task<ColumnDto> GetByIdAsync(Guid id);


    [Get("/column/by-board/{boardId}")]
    Task<List<ColumnDto>> GetByBoardIdAsync(Guid boardId);


    [Post("/column")]
    Task<Guid> CreateAsync([Body] CreateColumnDto command);


    [Put("/column/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateColumnDto command);


    [Delete("/column/{id}")]
    Task DeleteAsync(Guid id);
}
