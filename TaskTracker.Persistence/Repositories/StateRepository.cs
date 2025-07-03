using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Persistence.Repositories.Base;

namespace TaskTracker.Persistence.Repositories;

public class StateRepository : BaseRepository<State, Guid>, IStateRepository
{
    public StateRepository(ApplicationDbContext context) : base(context) { }

    public async Task<State?> GetByCardIdAsync(Guid cardId)
    {
        return await _dbSet.FindAsync(cardId);
    }
}
