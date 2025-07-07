using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.User.Queries.GetByEmail;

public class GetUserByEmailQuery : IRequest<UserDto>
{
    public string Email { get; set; }
}
