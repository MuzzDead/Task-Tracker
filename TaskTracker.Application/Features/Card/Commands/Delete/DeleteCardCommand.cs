using MediatR;

namespace TaskTracker.Application.Features.Card.Commands.Delete;

public class DeleteCardCommand : IRequest
{
    public Guid Id { get; set; }

    public DeleteCardCommand(Guid id)
    {
        Id = id;
    }
}
