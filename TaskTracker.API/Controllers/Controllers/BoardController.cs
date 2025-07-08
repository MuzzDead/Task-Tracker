using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Features.Board.Commands.Archive;
using TaskTracker.Application.Features.Board.Commands.Create;
using TaskTracker.Application.Features.Board.Commands.Delete;
using TaskTracker.Application.Features.Board.Commands.RemoveUser;
using TaskTracker.Application.Features.Board.Commands.Update;
using TaskTracker.Application.Features.Board.Queries.GetById;
using TaskTracker.Domain.Entities;

namespace TaskTracker.API.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BoardController : ControllerBase
{
    private readonly IMediator _mediator;
    public BoardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<BoardDto> GetBoardByUserId([FromQuery] GetBoardByIdQuery query)
    {
        var baord = await _mediator.Send(query);

        return baord;
    }

    [HttpPost]
    public async Task<ActionResult<Board>> Create([FromBody] CreateBoardCommand command)
    {
        var board = await _mediator.Send(command);

        return Ok(board);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateBoardCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var command = new DeleteBoardCommand(id);

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPut("{id:guid}/archive")]
    public async Task<ActionResult> Archive(Guid id)
    {
        var command = new ArchiveBoardCommand(id);

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{userId:guid}/remove-user")]
    public async Task<ActionResult> RemoveUserFromBoard(Guid boardId, Guid userId)
    {
        var command = new RemoveUserFromBoardCommand(boardId, userId);

        await _mediator.Send(command);

        return NoContent();
    }
}
