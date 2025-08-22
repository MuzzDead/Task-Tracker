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

    public void UpdateRange(IEnumerable<Column> columns)
    {
        _dbSet.UpdateRange(columns);
    }

    public async Task ShiftIndexesRightAsync(Guid boardId, int fromIndex, CancellationToken cancellationToken)
    {
        await _dbSet
            .Where(c => c.BoardId == boardId && c.ColumnIndex >= fromIndex)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.ColumnIndex, c => c.ColumnIndex + 1), cancellationToken);
    }

    public async Task SetColumnIndexAsync(Guid columnId, int newIndex, CancellationToken cancellationToken)
    {
        await _dbSet
            .Where(c => c.Id == columnId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.ColumnIndex, newIndex), cancellationToken);
    }
}
