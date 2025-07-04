using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.Board.Commands.Update;

public class UpdateBoardCommandHandler : IRequestHandler<UpdateBoardCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public UpdateBoardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(UpdateBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var board = await uow.Boards.GetByIdAsync(request.Id);

        if (board == null)
        {
            throw new NotFoundException($"Board with ID {request.Id} not found");
        }

        board.Title = request.Title;
        board.Description = request.Description;
        board.UpdatedAt = DateTimeOffset.UtcNow;
        board.UpdatedBy = request.UpdatedBy.ToString();

        await uow.Boards.UpdateAsync(board);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
