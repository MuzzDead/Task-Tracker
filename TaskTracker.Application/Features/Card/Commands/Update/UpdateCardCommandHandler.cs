using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.Card.Commands.Update;

public class UpdateCardCommandHandler : IRequestHandler<UpdateCardCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public UpdateCardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(UpdateCardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var card = await uow.Cards.GetByIdAsync(request.Id);

        if (card == null)
        {
            throw new NotFoundException($"Card with ID {request.Id} not found");
        }

        card.Title = request.Title;
        card.ColumnId = request.ColumnId;
        card.RowIndex = request.RowIndex;
        card.UpdatedAt = DateTimeOffset.UtcNow;
        card.UpdatedBy = request.UpdatedBy.ToString();

        await uow.Cards.UpdateAsync(card);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
