using AutoMapper;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs.Attach;
using TaskTracker.Application.Features.Comment.Commands.Create;
using TaskTracker.Application.Features.Comment.Commands.Delete;
using TaskTracker.Application.Features.Comment.Commands.Update;
using TaskTracker.Application.Features.Comment.Queries.GetByCardId;
using TaskTracker.Application.Features.Comment.Queries.GetById;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public CommentController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var query = new GetCommentByIdQuery { Id = id };

        var comment = await _mediator.Send(query);

        return Ok(comment);
    }

    [HttpGet("card/{cardId}")]
    public async Task<IActionResult> GetByCardIdAsync(Guid cardId)
    {
        var query = new GetCommentsByCardIdQuery { CardId = cardId };

        var comments = await _mediator.Send(query);

        return Ok(comments);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromForm] CreateCommentRequest request)
    {
        var command = _mapper.Map<CreateCommentCommand>(request);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateCommentCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var command = new DeleteCommentCommand { Id = id };

        await _mediator.Send(command);

        return NoContent();
    }
}
