using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Domain.Common;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class TaskItem : BaseEntity, IAuditableEntity
{
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskPriority Priority { get; private set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; private set; }
    public int Order { get; private set; }
    public bool IsArchived { get; private set; } = false;

    public Guid BoardId { get; private set; }
    public Guid StateId { get; private set; }
    public Guid? AssigneeId { get; private set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }

    public virtual Board Board { get; private set; } = null!;
    public virtual TaskState State { get; private set; } = null!;
    public virtual User? Assignee { get; private set; }
    public virtual ICollection<Comment> Comments { get; private set; } = new List<Comment>();
    public virtual ICollection<TaskAttachment> Attachments { get; private set; } = new List<TaskAttachment>();

}
