using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Features.State.Command.Create;
using TaskTracker.Application.Features.State.Command.Delete;
using TaskTracker.Application.Features.State.Command.Update;
using TaskTracker.Application.Features.State.Queries.GetByCardId;
using TaskTracker.Application.Features.State.Queries.GetById;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StateController : ControllerBase
{
    private readonly IMediator _mediator;
    public StateController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StateDto>> GetStateById(Guid id)
    {
        var query = new GetStateByIdQuery { Id = id };

        var state = await _mediator.Send(query);
        
        return Ok(state);
    }

    [HttpGet("card/{cardId:guid}")]
    public async Task<ActionResult<StateDto>> GetStateByCardId(Guid cardId)
    {
        var query = new GetStateByCardIdQuery { CardId = cardId };
        
        var state = await _mediator.Send(query);
        
        return Ok(state);
    }

    [HttpPost]
    public async Task<ActionResult> CreateState([FromBody] CreateStateCommand command)
    {
        var state = await _mediator.Send(command);
        
        return Ok(state);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateState(Guid id, [FromBody] UpdateStateCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        await _mediator.Send(command);
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteState(Guid id)
    {
        var command = new DeleteStateCommand { Id = id };
        
        await _mediator.Send(command);
        
        return NoContent();
    }
}
