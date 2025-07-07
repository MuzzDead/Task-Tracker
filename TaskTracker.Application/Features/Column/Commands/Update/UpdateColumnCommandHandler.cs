using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.Column.Commands.Update;

public class UpdateColumnCommandHandler : IRequestHandler<UpdateColumnCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public UpdateColumnCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(UpdateColumnCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var column = await uow.Columns.GetByIdAsync(request.Id);
        if (column == null)
            throw new NotFoundException($"Column with Id {request.Id} not found.");

        column.Title = request.Title;
        column.ColumnIndex = request.ColumnIndex;
        column.UpdatedAt = DateTimeOffset.UtcNow;
        column.UpdatedBy = "system";

        await uow.Columns.UpdateAsync(column);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
