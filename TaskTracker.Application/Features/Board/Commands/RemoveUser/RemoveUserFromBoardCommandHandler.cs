using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.Board.Commands.RemoveUser;

public class RemoveUserFromBoardCommandHandler : IRequestHandler<RemoveUserFromBoardCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public RemoveUserFromBoardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(RemoveUserFromBoardCommand request, CancellationToken cancellationToken)
    {
        using var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork();

        var board = await unitOfWork.Boards.GetByIdAsync(request.BoardId);

        if (board == null)
        {
            throw new NotFoundException($"Board with ID {request.BoardId} not found");
        }

        await unitOfWork.Boards.RemoveUserAsync(request.BoardId, request.UserId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
