using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.State.Command.Update;

public class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public UpdateStateCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(UpdateStateCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var state = await uow.States.GetByCardIdAsync(request.CardId);
        if (state == null)
            throw new NotFoundException($"State with ID {request.CardId} was not found.");

        if (!string.IsNullOrEmpty(request.Description))
            state.Description = request.Description;

        if (request.IsCompleted.HasValue)
            state.IsCompleted = request.IsCompleted.Value;

        if (request.Priority.HasValue)
            state.Priority = request.Priority.Value;

        if (request.Deadline.HasValue)
            state.Deadline = request.Deadline.Value;

        if (request.AssigneeId.HasValue)
        {
            state.AssigneeId = request.AssigneeId.Value == Guid.Empty ? null : request.AssigneeId.Value;
        }

        state.UpdatedAt = DateTimeOffset.UtcNow;

        if (request.UpdatedBy.HasValue)
            state.UpdatedBy = request.UpdatedBy.Value.ToString();

        await uow.States.UpdateAsync(state);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
