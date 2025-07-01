using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetCommentsByTaskIdAsync(Guid taskId);
}
