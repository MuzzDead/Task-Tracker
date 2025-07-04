using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.Board.Commands.Archive;

public class ArchiveBoardCommandHandler : IRequestHandler<ArchiveBoardCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public ArchiveBoardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(ArchiveBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var board = await uow.Boards.GetByIdAsync(request.Id);

        if (board == null)
        {
            throw new NotFoundException($"Board with ID {request.Id} not found");
        }

        if (board.IsArchived)
        {
            throw new InvalidOperationException("Board is already archived");
        }

        await uow.Boards.ArchiveAsync(request.Id);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
