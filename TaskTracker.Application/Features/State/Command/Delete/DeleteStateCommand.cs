using MediatR;

namespace TaskTracker.Application.Features.State.Command.Delete;

public class DeleteStateCommand : IRequest
{
    public Guid Id { get; set; }
}
