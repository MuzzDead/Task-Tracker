using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Persistence.Repositories.Base;
using TaskTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
}
