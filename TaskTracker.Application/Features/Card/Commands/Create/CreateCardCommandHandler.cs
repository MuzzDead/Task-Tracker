using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;

namespace TaskTracker.Application.Features.Card.Commands.Create;

public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, Guid>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public CreateCardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var maxRowIndex = await uow.Cards.GetMaxRowIndexByColumnIdAsync(request.ColumnId);

        var card = new Domain.Entities.Card
        {
            Title = request.Title,
            ColumnId = request.ColumnId,
            RowIndex = maxRowIndex + 1,
            CreatedBy = request.CreatedBy.ToString() ?? "system"
        };

        await uow.Cards.AddAsync(card);
        await uow.SaveChangesAsync(cancellationToken);

        return card.Id;
    }
}
