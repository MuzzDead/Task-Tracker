using MediatR;
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
public class CardController : ControllerBase
{
    private readonly IMediator _mediator;
    public CardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateCardCommand command)
    {
        var card = await _mediator.Send(command);

        return Ok(card);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, UpdateCardCommand command)
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
        var command = new DeleteCardCommand { Id = id };

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<CardDto> GetCardById([FromQuery] GetCardByIdQuery query)
    {
        var card = await _mediator.Send(query);

        return card;
    }

    [HttpGet("{columnId:guid}")]
    public async Task<IEnumerable<CardDto>> GetCardByColumnId([FromQuery] GetCardsByColumnIdQuery query)
    {
        var cards = await _mediator.Send(query);

        return cards;
    }
}
