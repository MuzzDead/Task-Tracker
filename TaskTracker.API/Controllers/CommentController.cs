using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Features.Comment.Commands.Create;
using TaskTracker.Application.Features.Comment.Commands.Delete;
using TaskTracker.Application.Features.Comment.Commands.Update;
using TaskTracker.Application.Features.Comment.Queries.GetByCardId;
using TaskTracker.Application.Features.Comment.Queries.GetById;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly IMediator _mediator;
    public CommentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCommentById(Guid id)
    {
        var query = new GetCommentByIdQuery { Id = id };

        var comment = await _mediator.Send(query);

        return Ok(comment);
    }

    [HttpGet("card/{cardId}")]
    public async Task<IActionResult> GetCommentsByCardId(Guid cardId)
    {
        var query = new GetCommentsByCardIdQuery { CardId = cardId };

        var comments = await _mediator.Send(query);

        return Ok(comments);
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentCommand command)
    {
        var comment = await _mediator.Send(command);

        return Ok(comment);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        var command = new DeleteCommentCommand { Id = id };

        await _mediator.Send(command);

        return NoContent();
    }
}
