using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface IBoardRoleRepository : IRepository<BoardRole, Guid>
{
}
