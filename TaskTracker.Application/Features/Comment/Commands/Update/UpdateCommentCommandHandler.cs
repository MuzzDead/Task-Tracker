using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.Comment.Commands.Update;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public UpdateCommentCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var comment = await uow.Comments.GetByIdAsync(request.Id);

        if (comment == null)
        {
            throw new NotFoundException($"Comment with ID {request.Id} not found");
        }

        comment.Text = request.Text;
        comment.UpdatedAt = DateTimeOffset.UtcNow;
        comment.UpdatedBy = request.UpdatedBy.ToString();

        await uow.Comments.UpdateAsync(comment);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
