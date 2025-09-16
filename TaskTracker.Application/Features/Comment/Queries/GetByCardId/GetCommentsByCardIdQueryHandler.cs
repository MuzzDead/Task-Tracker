using AutoMapper;
using MediatR;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Storage;

namespace TaskTracker.Application.Features.Comment.Queries.GetByCardId;

public class GetCommentsByCardIdQueryHandler : IRequestHandler<GetCommentsByCardIdQuery, IEnumerable<CommentDto>>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IMapper _mapper;
    private readonly IBlobService _blobService;
    public GetCommentsByCardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper, IBlobService blobService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _mapper = mapper;
        _blobService = blobService;
    }

    public async Task<IEnumerable<CommentDto>> Handle(GetCommentsByCardIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.CreateUnitOfWork();

        var comments = await uow.Comments.GetByCardIdAsync(request.CardId);
        var commentDtos = _mapper.Map<List<CommentDto>>(comments);

        foreach (var commentDto in commentDtos)
        {
            var attachments = await uow.CommentAttachments.GetByCommentIdAsync(commentDto.Id);
            var attachmentDtos = _mapper.Map<List<CommentAttachmentDto>>(attachments);

            foreach (var attachmentDto in attachmentDtos)
            {
                var attachment = attachments.First(a => a.Id == attachmentDto.Id);
                attachmentDto.DownloadUrl = _blobService.GenerateSasToken(attachment.BlobId, "files", 60);
            }

            commentDto.Attachments = attachmentDtos;
        }

        return commentDtos;
    }
}
