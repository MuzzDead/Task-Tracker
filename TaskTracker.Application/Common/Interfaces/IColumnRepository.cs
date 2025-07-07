using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface IColumnRepository : IRepository<Column, Guid>
{
    Task<IEnumerable<Column>> GetByBoardIdAsync(Guid boardId);
}
