using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;

namespace TaskTracker.Application.Features.Board.Commands.Create;

public class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, Guid>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    public CreateBoardCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();
        await uow.BeginTransactionAsync();

        try
        {
            var board = new Domain.Entities.Board
            {
                Title = request.Title,
                Description = request.Description,
                CreatedBy = request.UserId.ToString()
            };

            await uow.Boards.CreateAsync(board, request.UserId, request.UserRole);
            await uow.SaveChangesAsync();

            var boardRole = new Domain.Entities.BoardRole
            {
                BoardId = board.Id,
                UserId = request.UserId,
                Role = request.UserRole
            };

            await uow.BoardRoles.AddAsync(boardRole);
            await uow.SaveChangesAsync();

            await uow.CommitTransactionAsync();
            return board.Id;
        }
        catch (Exception ex)
        {
            await uow.RollbackTransactionAsync();
            throw;
        }
    }
}
