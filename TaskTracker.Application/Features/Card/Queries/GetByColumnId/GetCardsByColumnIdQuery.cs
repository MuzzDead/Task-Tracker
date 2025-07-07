using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Card.Queries.GetByColumnId;

public class GetCardsByColumnIdQuery : IRequest<IEnumerable<CardDto>>
{
    public Guid ColumnId { get; set; }

    public GetCardsByColumnIdQuery(Guid columnId)
    {
        ColumnId = columnId;
    }
}
