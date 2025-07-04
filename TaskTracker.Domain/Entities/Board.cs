using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class Board : ArchivableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}