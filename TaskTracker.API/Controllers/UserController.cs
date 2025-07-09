using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Features.User.Command.Create;
using TaskTracker.Application.Features.User.Command.Delete;
using TaskTracker.Application.Features.User.Command.Update;
using TaskTracker.Application.Features.User.Queries.GetByEmail;
using TaskTracker.Application.Features.User.Queries.GetById;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery { Id = id });
        
        return Ok(user);
    }

    [HttpGet("by-email")]
    public async Task<ActionResult<UserDto>> GetUserByEmail([FromQuery] string email)
    {
        var user = await _mediator.Send(new GetUserByEmailQuery { Email = email });
        
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserCommand command)
    {
        var user = await _mediator.Send(command);
        
        return Ok(user);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        await _mediator.Send(command);
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand { Id = id });
        
        return NoContent();
    }
}
