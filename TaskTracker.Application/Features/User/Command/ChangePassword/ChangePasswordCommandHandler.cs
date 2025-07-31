using MediatR;
using TaskTracker.Application.Common.Interfaces.Auth;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.User.Command.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IPasswordHasher _passwordHasher;
    public ChangePasswordCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IPasswordHasher passwordHasher)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var user = await uow.Users.GetByIdAsync(request.Id);
        if (user == null)
            throw new NotFoundException($"User with ID {request.Id} was not found.");

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials");

        user.PasswordHash = _passwordHasher.Generate(request.NewPassword);

        await uow.Users.UpdateAsync(user);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
