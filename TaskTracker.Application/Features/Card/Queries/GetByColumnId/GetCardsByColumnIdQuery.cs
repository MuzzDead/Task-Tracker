using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Card.Queries.GetByColumnId;

public class GetCardsByColumnIdQuery : IRequest<IEnumerable<CardDto>>
{
    public Guid ColumnId { get; set; }
}
