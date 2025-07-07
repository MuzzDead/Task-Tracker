using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.Comment.Commands.Delete;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public DeleteCommentCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var comment = await uow.Comments.GetByIdAsync(request.Id);

        if (comment == null)
        {
            throw new NotFoundException($"Comment with ID {request.Id} not found");
        }

        await uow.Comments.DeleteAsync(request.Id);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
