using MediatR;

namespace TaskTracker.Application.Features.Column.Commands.Update;

public class UpdateColumnCommand : IRequest
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ColumnIndex { get; set; }
}
