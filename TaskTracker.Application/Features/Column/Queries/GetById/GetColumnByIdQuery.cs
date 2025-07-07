using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Column.Queries.GetById;

public class GetColumnByIdQuery : IRequest<ColumnDto>
{
    public Guid Id { get; set; }
}
