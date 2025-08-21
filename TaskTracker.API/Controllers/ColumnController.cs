using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Features.Column.Commands.Create;
using TaskTracker.Application.Features.Column.Commands.Delete;
using TaskTracker.Application.Features.Column.Commands.Move;
using TaskTracker.Application.Features.Column.Commands.Update;
using TaskTracker.Application.Features.Column.Queries.GetByBoardId;
using TaskTracker.Application.Features.Column.Queries.GetById;

namespace TaskTracker.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ColumnController : ControllerBase
{
    private readonly IMediator _mediator;

    public ColumnController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("by-id/{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var column = await _mediator.Send(new GetColumnByIdQuery { Id = id });
        return Ok(column);
    }

    [HttpGet("by-board/{boardId:guid}")]
    public async Task<IActionResult> GetByBoardIdAsync(Guid boardId)
    {
        var columns = await _mediator.Send(new GetColumnsByBoardIdQuery { BoardId = boardId });
        return Ok(columns);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateColumnCommand command)
    {
        var columnId = await _mediator.Send(command);
        return Ok(columnId);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateColumnCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _mediator.Send(new DeleteColumnCommand { Id = id });
        return NoContent();
    }

    [HttpPut("move/{id:guid}")]
    public async Task<IActionResult> MoveAsync(Guid id, [FromBody]MoveColumnCommand command)
    {
        if (id != command.ColumnId)
            return BadRequest("Route ID does not match body ID.");

        await _mediator.Send(command);

        return NoContent();
    }
}

