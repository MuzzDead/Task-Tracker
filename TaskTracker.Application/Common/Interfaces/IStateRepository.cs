using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface IStateRepository : IRepository<State, Guid>
{
    Task<State?> GetByCardIdAsync(Guid cardId);
}
