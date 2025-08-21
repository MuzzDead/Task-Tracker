using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface ICardRepository : IRepository<Card, Guid>
{
    Task<IEnumerable<Card>> GetByColumnIdAsync(Guid columnId);
    Task<int> GetMaxRowIndexByColumnIdAsync(Guid columnId);
    Task MoveAsync(Guid cardId, Guid columnId, int index);
}
