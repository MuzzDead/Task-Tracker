﻿using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.User.Command.LoginUser;

public class LoginUserCommand : IRequest<AuthResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
