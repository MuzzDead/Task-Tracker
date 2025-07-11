using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Board.Queries.GetByUserId;

public class GetBoardsByUserIdQuery : IRequest<IEnumerable<BoardDto>>
{
    public Guid UserId { get; set; }
}
