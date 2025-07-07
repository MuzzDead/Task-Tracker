using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.User.Command.Delete;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public DeleteUserCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var user = await uow.Users.GetByIdAsync(request.Id);

        if (user == null)
        {
            throw new NotFoundException($"Columns with ID {request.Id} not found");
        }

        await uow.Users.DeleteAsync(request.Id);
        await uow.SaveChangesAsync();
    }
}
