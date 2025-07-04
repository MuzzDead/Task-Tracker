using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface ICommentRepository : IRepository<Comment, Guid>
{
    Task<IEnumerable<Comment>> GetByCardId(Guid cardId);
}
