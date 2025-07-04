namespace TaskTracker.Domain.Common;

public abstract class ArchivableEntity : BaseAuditableEntity
{
    public bool IsArchived { get; set; }
    public DateTimeOffset ArchivedAt { get; set; }
}
