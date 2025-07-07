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

        var state = await uow.States.GetByIdAsync(request.Id);
        if (state == null)
            throw new NotFoundException($"User with ID {request.Id} was not found.");

        state.Description = request.Description;
        state.Status = request.Status;
        state.Priority = request.Priority;

        await uow.States.UpdateAsync(state);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
