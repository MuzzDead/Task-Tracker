using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Column.Queries.GetByBoardId;

public class GetColumnsByBoardIdQuery : IRequest<IEnumerable<ColumnDto>>
{
    public Guid BoardId { get; set; }
}
