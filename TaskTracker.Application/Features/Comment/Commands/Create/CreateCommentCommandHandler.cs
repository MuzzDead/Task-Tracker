using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.Exceptions;
using TaskTracker.Application.Storage;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Features.Comment.Commands.Create;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IBlobService _blobService;

    public CreateCommentCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IBlobService blobService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _blobService = blobService;
    }

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        if (request.Attachments.Count > FileValidation.MaxFilesPerComment)
            throw new ValidationException($"Maximum {FileValidation.MaxFilesPerComment} files allowed");

        foreach (var attachment in request.Attachments)
        {
            if (!FileValidation.IsValidContentType(attachment.ContentType))
                throw new ValidationException($"File type {attachment.ContentType} is not allowed");

            if (!FileValidation.IsValidSize(attachment.Size))
                throw new ValidationException($"File size must be between 1 byte and {FileValidation.MaxFileSize} bytes");
        }

        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var comment = new Domain.Entities.Comment
        {
            Text = request.Text,
            CardId = request.CardId,
            UserId = request.UserId,
            CreatedBy = request.CreatedBy
        };

        await uow.Comments.AddAsync(comment);

        var uploadedBlobIds = new List<Guid>();
        try
        {
            foreach (var attachment in request.Attachments)
            {
                var blobId = await _blobService.UploadAsync(attachment.Content, attachment.ContentType);
                uploadedBlobIds.Add(blobId);

                var attachmentEntity = new CommentAttachment
                {
                    CommentId = comment.Id,
                    BlobId = blobId,
                    FileName = attachment.FileName,
                    ContentType = attachment.ContentType,
                    Size = attachment.Size,
                    CreatedBy = request.CreatedBy
                };

                await uow.CommentAttachments.AddAsync(attachmentEntity);
            }

            await uow.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            foreach (var blobId in uploadedBlobIds)
            {
                await _blobService.DeleteAsync(blobId);
            }
            throw;
        }

        return comment.Id;
    }
}
