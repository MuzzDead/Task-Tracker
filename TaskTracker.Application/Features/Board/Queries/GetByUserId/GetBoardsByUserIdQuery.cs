using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Board.Queries.GetByUserId;

public class GetBoardsByUserIdQuery : IRequest<IEnumerable<BoardDto>>
{
    public Guid UserId { get; set; }
}
