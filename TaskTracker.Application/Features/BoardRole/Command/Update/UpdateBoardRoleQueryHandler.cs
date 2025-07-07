using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.BoardRole.Command.Update;

public class UpdateBoardRoleQueryHandler : IRequestHandler<UpdateBoardRoleQuery>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public UpdateBoardRoleQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(UpdateBoardRoleQuery request, CancellationToken cancellationToken)
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
