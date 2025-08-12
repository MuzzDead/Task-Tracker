using TaskTracker.Domain.Common;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class State : BaseAuditableEntity
{
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public Priority Priority { get; set; } = Priority.Low;
    public DateTimeOffset? Deadline { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid CardId { get; set; }
}