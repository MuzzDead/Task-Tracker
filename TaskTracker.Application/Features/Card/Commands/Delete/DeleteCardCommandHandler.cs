using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.Card.Commands.Delete;

public class DeleteCardCommandHandler : IRequestHandler<DeleteCardCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public DeleteCardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(DeleteCardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var card = await uow.Cards.GetByIdAsync(request.Id);

        if (card == null)
        {
            throw new NotFoundException($"Card with ID {request.Id} not found");
        }

        await uow.Cards.DeleteAsync(request.Id);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
