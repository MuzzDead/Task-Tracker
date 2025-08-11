using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Board.Queries.Search;

public class SearchBoardsQuery : IRequest<IEnumerable<BoardDto>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
