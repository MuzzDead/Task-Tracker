using MediatR;

namespace TaskTracker.Application.Features.Column.Commands.Move;

public class MoveColumnCommand : IRequest
{
    public Guid ColumnId { get; set; }
    public Guid BeforeColumnId { get; set; }
}
