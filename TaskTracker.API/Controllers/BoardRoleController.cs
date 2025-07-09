using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Features.BoardRole.Command.Create;
using TaskTracker.Application.Features.BoardRole.Command.Delete;
using TaskTracker.Application.Features.BoardRole.Command.Update;
using TaskTracker.Application.Features.BoardRole.Queries.GetAll;
using TaskTracker.Application.Features.BoardRole.Queries.GetById;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardRoleController : ControllerBase
{
    private readonly IMediator _mediator;
    public BoardRoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBoardRoles()
    {
        var query = new GetAllBoardRoleQuery();
        
        var roles = await _mediator.Send(query);
        
        return Ok(roles);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBoardRoleById(Guid id)
    {
        var query = new GetBoardRoleByIdQuery { Id = id };
        
        var role = await _mediator.Send(query);
        
        return Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBoardRole([FromBody] CreateBoardRoleCommand command)
    {
        var role = await _mediator.Send(command);
        
        return Ok(role);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBoardRole(Guid id, [FromBody] UpdateBoardRoleCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        await _mediator.Send(command);
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBoardRole(Guid id)
    {
        var command = new DeleteBoardRoleCommand { Id = id };
        
        await _mediator.Send(command);
        
        return NoContent();
    }
}
