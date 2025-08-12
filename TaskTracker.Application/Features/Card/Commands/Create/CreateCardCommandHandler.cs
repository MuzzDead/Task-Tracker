using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Domain.Enums;

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
        await uow.BeginTransactionAsync();

        try
        {
            var maxRowIndex = await uow.Cards.GetMaxRowIndexByColumnIdAsync(request.ColumnId);

            var card = new Domain.Entities.Card
            {
                Title = request.Title,
                ColumnId = request.ColumnId,
                RowIndex = maxRowIndex + 1,
                CreatedBy = request.CreatedBy.ToString()
            };

            await uow.Cards.AddAsync(card);
            await uow.SaveChangesAsync(cancellationToken);

            var state = new Domain.Entities.State
            {
                Description = string.Empty,
                IsCompleted = false,
                Priority = Priority.Low,
                Deadline = null,
                AssigneeId = null,
                CardId = card.Id,
                CreatedBy = request.CreatedBy.ToString()
            };

            await uow.States.AddAsync(state);
            await uow.SaveChangesAsync(cancellationToken);

            await uow.CommitTransactionAsync();
            return card.Id;
        }
        catch (Exception ex)
        {
            await uow.RollbackTransactionAsync();
            throw;
        }
    }
}