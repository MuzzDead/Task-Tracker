using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class TaskState : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public Guid BoardId { get; set; }
}