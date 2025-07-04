using TaskTracker.Domain.Enums;


namespace TaskTracker.Application.DTOs;

public class StateDto
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public Status Status { get; set; }
    public Priority Priority { get; set; }
    public Guid CardId { get; set; }
}
