using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.Column.Commands.Move;

public class MoveColumnCommandHandler : IRequestHandler<MoveColumnCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public MoveColumnCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(MoveColumnCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();
        
        await uow.BeginTransactionAsync();

        try
        {
            var currentColumn = await uow.Columns.GetByIdAsync(request.ColumnId);
            if (currentColumn == null)
                throw new NotFoundException($"Column {request.ColumnId} not found");

            var targetColumn = await uow.Columns.GetByIdAsync(request.BeforeColumnId);
            if (targetColumn == null)
                throw new NotFoundException($"Column {request.BeforeColumnId} not found");

            if (currentColumn.Id == targetColumn.Id)
                throw new InvalidOperationException("Cannot move column before itself");

            await uow.Columns.ShiftIndexesRightAsync(currentColumn.BoardId, targetColumn.ColumnIndex, cancellationToken);

            await uow.Columns.SetColumnIndexAsync(currentColumn.Id, targetColumn.ColumnIndex, cancellationToken);

            await uow.CommitTransactionAsync();
        }
        catch
        {
            await uow.RollbackTransactionAsync();
            throw;
        }
    }
}