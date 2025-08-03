using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Features.Card.Commands.Create;
using TaskTracker.Application.Features.Card.Commands.Delete;
using TaskTracker.Application.Features.Card.Commands.Update;
using TaskTracker.Application.Features.Card.Queries.GetByColumnId;
using TaskTracker.Application.Features.Card.Queries.GetById;

namespace TaskTracker.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CardController : ControllerBase
{
    private readonly IMediator _mediator;
    public CardController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAsync([FromBody] CreateCardCommand command)
    {
        var card = await _mediator.Send(command);

        return Ok(card);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateCardCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _mediator.Send(new DeleteCardCommand { Id = id });

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CardDto>> GetByIdAsync(Guid id)
    {
        var query = new GetCardByIdQuery { Id = id };

        var card = await _mediator.Send(query);

        return Ok(card);
    }

    [HttpGet("column/{columnId:guid}")]
    public async Task<ActionResult<IEnumerable<CardDto>>> GetByColumnIdAsync(Guid columnId)
    {
        var query = new GetCardsByColumnIdQuery { ColumnId = columnId };

        var cards = await _mediator.Send(query);

        return Ok(cards);
    }
}