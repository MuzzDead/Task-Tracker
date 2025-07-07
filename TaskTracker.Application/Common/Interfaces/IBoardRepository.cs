using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Common.Interfaces;

public interface IBoardRepository : IRepository<Board, Guid>
{
    Task CreateAsync(Guid boardId, Guid userId, UserRole userRole);
    Task RemoveUserAsync(Guid boardId,Guid userId);
    Task ArchiveAsync(Guid boardId);
}
