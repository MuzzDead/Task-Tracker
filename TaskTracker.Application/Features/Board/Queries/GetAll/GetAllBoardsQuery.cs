using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Board.Queries.GetAll;

public class GetAllBoardsQuery : IRequest<IEnumerable<BoardDto>>
{
    public bool IncludeArchived { get; set; } = false;
}
