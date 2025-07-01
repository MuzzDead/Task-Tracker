using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Interfaces;

public interface ITaskRepository : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetTasksByBoardIdAsync(Guid boardId);
    Task<IEnumerable<TaskItem>> GetTasksAssignedToUserAsync(Guid userId);
}
