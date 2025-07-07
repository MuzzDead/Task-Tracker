using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;

namespace TaskTracker.Application.Features.BoardRole.Command.Create;

public class CreateBoardRoleCommandHandler : IRequestHandler<CreateBoardRoleCommand, Guid>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public CreateBoardRoleCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Handle(CreateBoardRoleCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var role = new Domain.Entities.BoardRole
        {
            UserId = request.UserId,
            BoardId = request.BoardId,
            Role = request.Role
        };

        await uow.BoardRoles.AddAsync(role);
        await uow.SaveChangesAsync(cancellationToken);

        return role.Id;
    }
}
