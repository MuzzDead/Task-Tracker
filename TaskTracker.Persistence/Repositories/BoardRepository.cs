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
            await _context.SaveChangesAsync();
        }
    }

    public async Task CreateAsync(Board board, Guid userId, UserRole userRole)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _dbSet.AddAsync(board);

            var boardRole = new BoardRole
            {
                BoardId = board.Id,
                UserId = userId,
                Role = userRole
            };

            await _context.BoardRoles.AddAsync(boardRole);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task RemoveUserAsync(Guid boardId, Guid userId)
    {
        var boardRole = await _context.BoardRoles
            .FirstOrDefaultAsync(br => br.BoardId == boardId && br.UserId == userId);

        if (boardRole != null)
        {
            _context.BoardRoles.Remove(boardRole);
            await _context.SaveChangesAsync();
        }
    }
}
