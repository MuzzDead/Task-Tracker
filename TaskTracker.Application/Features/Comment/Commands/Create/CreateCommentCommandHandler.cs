using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;

namespace TaskTracker.Application.Features.Comment.Commands.Create;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public CreateCommentCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var comment = new Domain.Entities.Comment
        {
            Text = request.Text,
            CardId = request.CardId,
            UserId = request.UserId,
            CreatedBy = request.UserId.ToString()
        };

        await uow.Comments.AddAsync(comment);
        await uow.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}
