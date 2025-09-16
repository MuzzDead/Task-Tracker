using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Storage;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Features.Comment.Commands.Delete;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IBlobService _blobService;

    public DeleteCommentCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IBlobService blobService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _blobService = blobService;
    }

    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var comment = await uow.Comments.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Comment with ID {request.Id} not found");


        var attachments = await uow.CommentAttachments.GetByCommentIdAsync(request.Id);

        foreach (var attachment in attachments)
        {
            await _blobService.DeleteAsync(attachment.BlobId, "files");
        }

        await uow.CommentAttachments.DeleteByCommentIdAsync(request.Id);

        await uow.Comments.DeleteAsync(request.Id);
        await uow.SaveChangesAsync(cancellationToken);
    }
}
