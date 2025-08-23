using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Features.User.Command.ChangePassword;
using TaskTracker.Application.Features.User.Command.Delete;
using TaskTracker.Application.Features.User.Command.DeleteAvatar;
using TaskTracker.Application.Features.User.Command.RegisterUser;
using TaskTracker.Application.Features.User.Command.Update;
using TaskTracker.Application.Features.User.Command.UploadAvatar;
using TaskTracker.Application.Features.User.Queries.GetAvatar;
using TaskTracker.Application.Features.User.Queries.GetByEmail;
using TaskTracker.Application.Features.User.Queries.GetById;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery { Id = id });

        return Ok(user);
    }

    [HttpGet("by-email")]
    public async Task<IActionResult> GetByEmailAsync([FromQuery] string email)
    {
        var user = await _mediator.Send(new GetUserByEmailQuery { Email = email });

        return Ok(user);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPut("{id:guid}/change-password")]
    public async Task<IActionResult> ChangePasswordAsync(Guid id, [FromBody] ChangePasswordCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match body ID.");

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _mediator.Send(new DeleteUserCommand { Id = id });

        return NoContent();
    }

    [HttpPost("{id:guid}/avatar")]
    public async Task<IActionResult> UploadAvatarAsync(Guid id, IFormFile avatar)
    {
        var command = new UploadAvatarCommand { UserId = id, Avatar = avatar };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}/avatar")]
    public async Task<IActionResult> DeleteAvatarAsync(Guid id)
    {
        var command = new DeleteAvatarCommand { UserId = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id:guid}/avatar")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvatarAsync(Guid id)
    {
        var fileResponse = await _mediator.Send(new GetUserAvatarQuery { UserId = id });
        return File(fileResponse.Stream, fileResponse.ContentType);
    }
}
