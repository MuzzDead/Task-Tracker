using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Features.Column.Commands.Create;
using TaskTracker.Application.Features.Column.Commands.Delete;
using TaskTracker.Application.Features.Column.Commands.Update;
using TaskTracker.Application.Features.Column.Queries.GetByBoardId;
using TaskTracker.Application.Features.Column.Queries.GetById;

namespace TaskTracker.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ColumnController : ControllerBase
{
    private readonly IMediator _mediator;

    public ColumnController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("by-id/{id:guid}")]
    public async Task<IActionResult> GetColumnById(Guid id)
    {
        var column = await _mediator.Send(new GetColumnByIdQuery { Id = id });
        return Ok(column);
    }

    [HttpGet("by-board/{boardId:guid}")]
    public async Task<IActionResult> GetColumnsByBoardId(Guid boardId)
    {
        var columns = await _mediator.Send(new GetColumnsByBoardIdQuery { BoardId = boardId });
        return Ok(columns);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateColumnCommand command)
    {
        var columnId = await _mediator.Send(command);
        return Ok(columnId);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateColumnCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteColumnCommand { Id = id });
        return NoContent();
    }
}

