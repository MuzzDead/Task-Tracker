using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Board.Queries.GetAll;

public class GetAllBoardsQuery : IRequest<IEnumerable<BoardDto>>
{
    public bool IncludeArchived { get; set; } = false;
}
