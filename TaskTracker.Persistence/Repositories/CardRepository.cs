using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Persistence.Repositories.Base;

namespace TaskTracker.Persistence.Repositories;

public class CardRepository : BaseRepository<Card, Guid>, ICardRepository
{
    public CardRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Card>> GetByColumnIdAsync(Guid columnId)
    {
        return await _dbSet
            .Where(c => c.ColumnId == columnId)
            .OrderBy(c => c.RowIndex)
            .ToListAsync();
    }

    public async Task<int> GetMaxRowIndexByColumnIdAsync(Guid columnId)
    {
        return await _dbSet
            .Where(c => c.ColumnId == columnId)
            .Select(c => c.RowIndex)
            .DefaultIfEmpty(-1)
            .MaxAsync();
    }
}
