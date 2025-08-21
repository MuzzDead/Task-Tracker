using MediatR;

namespace TaskTracker.Application.Features.Card.Commands.Move;

public class MoveCardCommand : IRequest
{
    public Guid CardId { get; set; }
    public Guid ColumnId { get; set; }
}
