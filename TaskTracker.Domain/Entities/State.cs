using TaskTracker.Domain.Common;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class State : BaseAuditableEntity
{
    public string Description { get; set; } = string.Empty;
    public Status Status { get; set; } = Status.Pending;
    public Priority Priority { get; set; } = Priority.Medium;
    public Guid CardId { get; set; }
}