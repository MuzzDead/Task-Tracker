using MediatR;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Features.State.Queries.GetByCardId;

public class GetStateByCardIdQuery : IRequest<StateDto>
{
    public Guid CardId { get; set; }
}
