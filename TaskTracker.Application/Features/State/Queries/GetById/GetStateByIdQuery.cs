using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.State.Queries.GetById;

public class GetStateByIdQuery : IRequest<StateDto>
{
    public Guid Id { get; set; }
}
