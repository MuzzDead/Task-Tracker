using MediatR;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Pagination;

namespace TaskTracker.Application.Features.Board.Queries.Search;

public class SearchBoardsQuery : IRequest<PagedResult<BoardDto>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public PagedRequest PagedRequest { get; set; } = new();
}
