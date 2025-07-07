using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.Column.Commands.Delete;

public class DeleteColumnCommandHandler : IRequestHandler<DeleteColumnCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public DeleteColumnCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(DeleteColumnCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var column = await uow.Columns.GetByIdAsync(request.Id);

        if (column == null)
        {
            throw new NotFoundException($"Columns with ID {request.Id} not found");
        }

        await uow.Columns.DeleteAsync(request.Id);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
