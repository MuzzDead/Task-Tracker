using MediatR;

namespace TaskTracker.Application.Features.User.Command.ChangePassword;

public class ChangePasswordCommand :IRequest
{
    public Guid Id { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}
