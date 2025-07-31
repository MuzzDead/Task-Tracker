using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface IBoardRoleRepository : IRepository<BoardRole, Guid>
{
    Task<IEnumerable<MemberDto>> GetMembersByBoardIdAsync(Guid boardId);
}
