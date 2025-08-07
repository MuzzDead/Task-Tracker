using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;
using TaskTracker.Persistence.Repositories.Base;

namespace TaskTracker.Persistence.Repositories;

public class BoardRoleRepository : BaseRepository<BoardRole, Guid>, IBoardRoleRepository
{
    public BoardRoleRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<MemberDto>> GetMembersByBoardIdAsync(Guid boardId)
    {
        return await _context.BoardRoles
            .Where(br => br.BoardId == boardId)
            .Join(_context.Users,
                  br => br.UserId,
                  u => u.Id,
                  (br, u) => new MemberDto
                  {
                      BoardRoleId = br.Id,
                      UserId = br.UserId,
                      BoardId = br.BoardId,
                      UserRole = br.Role,
                      Username = u.Username,
                      Email = u.Email
                  })
            .OrderByDescending(m => m.UserRole)
            .ThenBy(m => m.Username)
            .AsNoTracking()
            .ToListAsync();
    }
}
