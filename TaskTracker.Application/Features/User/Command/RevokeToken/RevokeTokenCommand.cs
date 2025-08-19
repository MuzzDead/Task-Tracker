using MediatR;

namespace TaskTracker.Application.Features.User.Command.RevokeToken;

public class RevokeTokenCommand : IRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
