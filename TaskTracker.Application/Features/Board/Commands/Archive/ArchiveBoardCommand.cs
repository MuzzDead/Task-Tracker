using MediatR;

namespace TaskTracker.Application.Features.Board.Commands.Archive;

public class ArchiveBoardCommand : IRequest
{
    public Guid Id { get; set; }

    public ArchiveBoardCommand(Guid id)
    {
        Id = id;
    }
}
