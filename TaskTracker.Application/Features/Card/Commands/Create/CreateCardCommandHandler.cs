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

        var card = new Domain.Entities.Card
        {
            Title = request.Title,
            ColumnId = request.ColumnId,
            RowIndex = request.RowIndex,
            CreatedBy = request.CreatedBy.ToString()
        };

        await uow.Cards.AddAsync(card);
        await uow.SaveChangesAsync(cancellationToken);

        return card.Id;
    }
}
