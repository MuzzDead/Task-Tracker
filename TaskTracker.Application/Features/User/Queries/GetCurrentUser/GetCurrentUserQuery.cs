using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.User.Queries.GetCurrentUser;

public class GetCurrentUserQuery : IRequest<UserDto>
{
}
