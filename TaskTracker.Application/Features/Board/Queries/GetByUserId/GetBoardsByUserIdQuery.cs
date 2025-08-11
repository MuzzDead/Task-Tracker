using MediatR;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Pagination;

namespace TaskTracker.Application.Features.Board.Queries.GetByUserId;

public class GetBoardsByUserIdQuery : IRequest<PagedResult<BoardDto>>
{
    public Guid UserId { get; set; }
    public PagedRequest PagedRequest { get; set; } = new();
}
