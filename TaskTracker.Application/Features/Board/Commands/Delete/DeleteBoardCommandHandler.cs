using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.Board.Commands.Delete;

public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public DeleteBoardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(DeleteBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var board = await uow.Boards.GetByIdAsync(request.Id);

        if (board == null)
        {
            throw new NotFoundException($"Board with ID {request.Id} not found");
        }

        await uow.Boards.DeleteAsync(request.Id);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
