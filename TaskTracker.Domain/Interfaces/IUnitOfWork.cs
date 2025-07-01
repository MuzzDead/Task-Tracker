using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Domain.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IBoardRepository Boards { get; }
    ITaskRepository Tasks { get; }
    ICommentRepository Comments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
