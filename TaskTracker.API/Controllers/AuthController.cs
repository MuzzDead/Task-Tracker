using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Features.User.Command.LoginUser;
using TaskTracker.Application.Features.User.Command.RegisterUser;
using TaskTracker.Application.Features.User.Queries.GetCurrentUser;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> RegisterAsync(RegisterUserCommand command)
    {
        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> LoginAsync(LoginUserCommand command)
    {
        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var response = await _mediator.Send(new GetCurrentUserQuery());
        return Ok(response);
    }
}
