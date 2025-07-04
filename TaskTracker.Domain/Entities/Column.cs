using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class Column : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public int ColumnIndex { get; set; }
    public Guid BoardId { get; set; }
}