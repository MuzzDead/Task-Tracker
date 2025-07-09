using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.Board.Queries.GetById;

public class GetBoardByIdQuery : IRequest<BoardDto?>
{
    public Guid Id { get; set; }
}
