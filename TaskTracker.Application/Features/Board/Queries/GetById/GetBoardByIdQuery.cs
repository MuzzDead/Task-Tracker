using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Board.Queries.GetById;

public class GetBoardByIdQuery : IRequest<BoardDto?>
{
    public Guid Id { get; set; }

    public GetBoardByIdQuery(Guid id)
    {
        Id = id;
    }
}
