using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Application.DTOs;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface ICardRepository : IRepository<Card>
{
    Task<IEnumerable<Card>> GetCardsByColumnIdAsync(Guid columnId);
}
