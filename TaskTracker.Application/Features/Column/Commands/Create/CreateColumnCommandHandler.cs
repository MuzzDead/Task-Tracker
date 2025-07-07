using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;

namespace TaskTracker.Application.Features.Column.Commands.Create;

public class CreateColumnCommandHandler : IRequestHandler<CreateColumnCommand, Guid>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public CreateColumnCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Handle(CreateColumnCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var column = new Domain.Entities.Column
        {
            Title = request.Title,
            BoardId = request.BoardId,
            ColumnIndex = request.ColumnIndex,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = "system"
        };

        await uow.Columns.AddAsync(column);
        await uow.SaveChangesAsync(cancellationToken);

        return column.Id;
    }
}
