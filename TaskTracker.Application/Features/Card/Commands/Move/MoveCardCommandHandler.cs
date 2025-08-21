using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.Card.Commands.Move;

public class MoveCardCommandHandler : IRequestHandler<MoveCardCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public MoveCardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(MoveCardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var card = await uow.Cards.GetByIdAsync(request.CardId);
        if (card == null)
            throw new NotFoundException($"Card with id {request.CardId} not found");

        int currentIndex = await uow.Cards.GetMaxRowIndexByColumnIdAsync(request.ColumnId);

        await uow.Cards.MoveAsync(
            request.CardId,
            request.ColumnId,
            currentIndex + 1);

        await uow.SaveChangesAsync(cancellationToken);
    }
}
