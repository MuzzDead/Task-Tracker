using TaskTracker.Domain.Enums;


namespace TaskTracker.Application.DTOs;

public class StateDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public Priority Priority { get; set; }
    public DateTimeOffset? Deadline { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid CardId { get; set; }
}
