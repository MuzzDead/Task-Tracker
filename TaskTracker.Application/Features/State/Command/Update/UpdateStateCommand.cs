using MediatR;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.State.Command.Update;

public class UpdateStateCommand : IRequest
{
    public Guid CardId { get; set; }
    public string? Description { get; set; }
    public bool? IsCompleted { get; set; }
    public Priority? Priority { get; set; }
    public DateTimeOffset? Deadline { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid? UpdatedBy { get; set; }
}