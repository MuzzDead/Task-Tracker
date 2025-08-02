using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;
using TaskTracker.Persistence.Repositories.Base;

namespace TaskTracker.Persistence.Repositories;

public class BoardRepository : BaseRepository<Board, Guid>, IBoardRepository
{
    public BoardRepository(ApplicationDbContext context) : base(context) { }

    public async Task ArchiveAsync(Guid boardId)
    {
        var board = await GetByIdAsync(boardId);
        if (board != null)
        {
            board.IsArchived = true;
            board.ArchivedAt = DateTimeOffset.UtcNow;
        }
    }

    public async Task CreateAsync(Board board, Guid userId, UserRole userRole)
    {
        await _dbSet.AddAsync(board);
    }

    public async Task<IEnumerable<Board>> GetByUserId(Guid userId)
    {
        var boards = await _dbSet
            .AsNoTracking()
            .Where(b => b.CreatedBy == userId.ToString()
                && !b.IsArchived)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return boards;
    }

    public async Task RemoveUserAsync(Guid boardId, Guid userId)
    {
        var boardRole = await _context.BoardRoles
            .FirstOrDefaultAsync(br => br.BoardId == boardId && br.UserId == userId);

        if (boardRole != null)
        {
            _context.BoardRoles.Remove(boardRole);
        }
    }
}
