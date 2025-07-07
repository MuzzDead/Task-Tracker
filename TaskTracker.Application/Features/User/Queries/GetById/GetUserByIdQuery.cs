using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.User.Queries.GetById;

public class GetUserByIdQuery : IRequest<UserDto>
{
    public Guid Id { get; set; }
}
