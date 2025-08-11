using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Application.DTOs.Pagination;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Common.Interfaces;

public interface IBoardRepository : IRepository<Board, Guid>
{
    Task CreateAsync(Board board, Guid userId, UserRole userRole);
    Task RemoveUserAsync(Guid boardId,Guid userId);
    Task ArchiveAsync(Guid boardId);
    Task<PagedResult<Board>> GetByUserIdAsync(Guid userId, PagedRequest request);
    Task<PagedResult<Board>> SearchAsync(string searchTerm, Guid userId, PagedRequest request);
}
