using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.User.Command.Create;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public CreateUserCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var isUnique = await uow.Users.IsEmailUniqueAsync(request.Email);
        if (!isUnique)
        {
            throw new ConflictException("Email is already in use");
        }

        var user = new Domain.Entities.User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = request.PasswordHash
        };

        await uow.Users.AddAsync(user);
        await uow.SaveChangesAsync();

        return user.Id;
    }
}
