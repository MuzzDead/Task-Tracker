using MediatR;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.State.Command.Create;

public class CreateStateCommand : IRequest<Guid>
{
    public string Description { get; set; } = string.Empty;
    public Status Status { get; set; } = Status.Pending;
    public Priority Priority { get; set; } = Priority.Medium;
    public Guid CardId { get; set; }
}
