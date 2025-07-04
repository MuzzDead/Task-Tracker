using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class Card : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public Guid ColumnId {  get; set; }
    public int RowIndex {  get; set; }
}