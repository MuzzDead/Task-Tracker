using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Persistence.Repositories.Base;

namespace TaskTracker.Persistence.Repositories;

public class ColumnRepository : BaseRepository<Column, Guid>, IColumnRepository
{
    public ColumnRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Column>> GetByBoardIdAsync(Guid boardId)
    {
        var columns = await _dbSet
            .Where(c => c.BoardId == boardId)
            .OrderBy(c => c.ColumnIndex)
            .ToListAsync();

        return columns;
    }
}
