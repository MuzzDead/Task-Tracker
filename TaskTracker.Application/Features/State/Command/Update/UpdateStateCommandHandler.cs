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

        if (request.Description != null)
            state.Description = request.Description;

        if (request.IsCompleted.HasValue)
            state.IsCompleted = request.IsCompleted.Value;

        if (request.Priority.HasValue)
            state.Priority = request.Priority.Value;

        if (request.Deadline != null)
            state.Deadline = request.Deadline;

        if (request.AssigneeId != null)
            state.AssigneeId = request.AssigneeId;

        state.UpdatedAt = DateTimeOffset.UtcNow;
        state.UpdatedBy = request.UpdatedBy.ToString();

        await uow.States.UpdateAsync(state);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
