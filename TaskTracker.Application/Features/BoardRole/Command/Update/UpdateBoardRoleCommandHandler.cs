using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.BoardRole.Command.Update;

public class UpdateBoardRoleCommandHandler : IRequestHandler<UpdateBoardRoleCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public UpdateBoardRoleCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(UpdateBoardRoleCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var role = await uow.BoardRoles.GetByIdAsync(request.Id);
        if (role == null)
        {
            throw new NotFoundException($"BoardRole with ID {request.Id} was not found.");
        }

        await uow.BoardRoles.UpdateAsync(role);
        await uow.SaveChangesAsync();
    }
}
