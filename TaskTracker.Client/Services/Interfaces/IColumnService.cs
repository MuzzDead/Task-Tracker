using Refit;
using TaskTracker.Client.DTOs.Column;

namespace TaskTracker.Client.Services.Interfaces;

public interface IColumnService
{
    [Get("/api/column/by-id/{id}")]
    Task<ColumnDto> GetByIdAsync(Guid id);

    
    [Get("/api/column/by-board/{boardId}")]
    Task<List<ColumnDto>> GetByBoardIdAsync(Guid boardId);

    
    [Post("/api/column")]
    Task<Guid> CreateAsync([Body] CreateColumnDto command);

    
    [Put("/api/column/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateColumnDto command);

    
    [Delete("/api/column/{id}")]
    Task DeleteAsync(Guid id);
}
