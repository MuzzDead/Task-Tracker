using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        var boardRole = new BoardRole
        {
            BoardId = board.Id,
            UserId = userId,
            Role = userRole
        };

        await _context.BoardRoles.AddAsync(boardRole);
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
