using MediatR;

namespace TaskTracker.Application.Features.Board.Commands.Update;

public class UpdateBoardCommand : IRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid UpdatedBy { get; set; }
}
