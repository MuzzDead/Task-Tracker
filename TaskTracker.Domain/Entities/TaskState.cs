using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class TaskState : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public string? Color { get; private set; }
    public Guid BoardId { get; private set; }
    public bool IsCompleted { get; private set; } = false;

    public virtual Board Board { get; private set; } = null!;
    public virtual ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();
}
