using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Domain.Enums;

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
            Description = request.Description ?? string.Empty,
            IsCompleted = request.IsCompleted ?? false,
            Priority = request.Priority ?? Priority.Low,
            Deadline = request.Deadline,
            AssigneeId = request.AssigneeId,
            CardId = request.CardId,
            CreatedBy = request.CreatedBy.ToString()
        };

        await uow.States.AddAsync(state);
        await uow.SaveChangesAsync(cancellationToken);

        return state.Id;
    }
}
