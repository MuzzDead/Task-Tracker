using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;

namespace TaskTracker.Application.Features.State.Command.Create;

public class CreateStateCommandHandler : IRequestHandler<CreateStateCommand, Guid>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public CreateStateCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Handle(CreateStateCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var state = new Domain.Entities.State
        {
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
            CardId = request.CardId
        };

        await uow.States.AddAsync(state);
        await uow.SaveChangesAsync();

        return state.Id;
    }
}
