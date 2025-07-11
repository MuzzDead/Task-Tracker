using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Features.Board.Commands.Archive;
using TaskTracker.Application.Features.Board.Commands.Create;
using TaskTracker.Application.Features.Board.Commands.Delete;
using TaskTracker.Application.Features.Board.Commands.RemoveUser;
using TaskTracker.Application.Features.Board.Commands.Update;
using TaskTracker.Application.Features.Board.Queries.GetById;
using TaskTracker.Application.Features.Board.Queries.GetByUserId;
using TaskTracker.Domain.Entities;

namespace TaskTracker.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BoardController : ControllerBase
{
    private readonly IMediator _mediator;
    public BoardController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BoardDto>> GetByIdAsync(Guid id)
    {
        var query = new GetBoardByIdQuery { Id = id };

        var board = await _mediator.Send(query);

        return Ok(board);
    }

    [HttpGet("by-user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<BoardDto>>> GetByUserId(Guid userId)
    {
        var query = new GetBoardsByUserIdQuery { UserId = userId };

        var boards = await _mediator.Send(query);

        return Ok(boards);
    }

    [HttpPost]
    public async Task<ActionResult<BoardDto>> CreateAsync([FromBody] CreateBoardCommand command)
    {
        var board = await _mediator.Send(command);

        return Ok(board);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateBoardCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _mediator.Send(new DeleteBoardCommand(id));

        return NoContent();
    }

    [HttpPut("{id:guid}/archive")]
    public async Task<IActionResult> ArchiveAsync(Guid id)
    {
        await _mediator.Send(new ArchiveBoardCommand(id));

        return NoContent();
    }

    [HttpDelete("{boardId:guid}/users/{userId:guid}")]
    public async Task<IActionResult> RemoveUserFromBoardAsunc(Guid boardId, Guid userId)
    {
        await _mediator.Send(new RemoveUserFromBoardCommand(boardId, userId));

        return NoContent();
    }
}
