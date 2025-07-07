using MediatR;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Features.State.Command.Update;

public class UpdateStateCommand : IRequest
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Status Status { get; set; }
    public Priority Priority { get; set; }
}
