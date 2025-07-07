using MediatR;

namespace TaskTracker.Application.Features.Column.Commands.Create;

public class CreateColumnCommand : IRequest<Guid>
{
    public string Title { get; set; } = string.Empty;
    public Guid BoardId { get; set; }
    public int ColumnIndex { get; set; }
}
