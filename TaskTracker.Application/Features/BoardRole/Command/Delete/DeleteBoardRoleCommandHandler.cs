using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.BoardRole.Command.Delete;

public class DeleteBoardRoleCommandHandler : IRequestHandler<DeleteBoardRoleCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public DeleteBoardRoleCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(DeleteBoardRoleCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var role = await uow.BoardRoles.GetByIdAsync(request.Id);
        if (role == null)
        {
            throw new NotFoundException($"BoardRole with ID {request.Id} was not found.");
        }

        await uow.BoardRoles.DeleteAsync(request.Id);
        await uow.SaveChangesAsync();
    }
}
