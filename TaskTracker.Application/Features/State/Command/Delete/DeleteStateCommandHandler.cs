using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.State.Command.Delete;

public class DeleteStateCommandHandler : IRequestHandler<DeleteStateCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public DeleteStateCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(DeleteStateCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var state = await uow.States.GetByIdAsync(request.Id);
        if (state == null)
        {
            throw new NotFoundException($"State with ID {request.Id} was not found.");
        }

        await uow.States.DeleteAsync(request.Id);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
