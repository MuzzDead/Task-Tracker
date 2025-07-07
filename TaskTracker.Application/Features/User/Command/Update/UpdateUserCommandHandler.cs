using MediatR;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.User.Command.Update;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public UpdateUserCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var user = await uow.Users.GetByIdAsync(request.Id);
        if (user == null)
            throw new NotFoundException($"User with ID {request.Id} was not found.");

        user.Username = request.Username;
        user.Email = request.Email;

        await uow.Users.UpdateAsync(user);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
