using MediatR;

namespace TaskTracker.Application.Features.Board.Commands.Delete;

public class DeleteBoardCommand : IRequest
{
    public Guid Id { get; set; }
    public DeleteBoardCommand(Guid id)
    {
        Id = id;
    }
}
