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
        var createdBoards = GetCreatedBoardsQuery(userId);
        var memberBoards = GetMemberBoardsQuery(userId);

        return await GetBoardsAsync(createdBoards, memberBoards);
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

    public async Task<IEnumerable<Board>> SearchAsync(string searchTerm, Guid userId)
    {
        var searchCondition = $"%{searchTerm}%";

        var createdBoards = GetCreatedBoardsQuery(userId)
            .Where(b => EF.Functions.Like(b.Title, searchCondition)
                       || (b.Description != null && EF.Functions.Like(b.Description, searchCondition)));

        var memberBoards = GetMemberBoardsQuery(userId)
            .Where(b => EF.Functions.Like(b.Title, searchCondition)
                       || (b.Description != null && EF.Functions.Like(b.Description, searchCondition)));

        return await GetBoardsAsync(createdBoards, memberBoards);
    }


    private IQueryable<Board> GetCreatedBoardsQuery(Guid userId)
    {
        return _dbSet
            .AsNoTracking()
            .Where(b => b.CreatedBy == userId.ToString() && !b.IsArchived);
    }

    private IQueryable<Board> GetMemberBoardsQuery(Guid userId)
    {
        return _dbSet
            .AsNoTracking()
            .Join(_context.Set<BoardRole>(),
                  board => board.Id,
                  boardRole => boardRole.BoardId,
                  (board, boardRole) => new { Board = board, BoardRole = boardRole })
            .Where(joined => joined.BoardRole.UserId == userId && !joined.Board.IsArchived)
            .Select(joined => joined.Board);
    }

    private async Task<IEnumerable<Board>> GetBoardsAsync(IQueryable<Board> createdBoards, IQueryable<Board> memberBoards)
    {
        return await createdBoards
            .Union(memberBoards)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }
}
