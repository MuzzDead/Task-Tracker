using AutoMapper;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.DTOs.Attach;
using TaskTracker.Application.Features.Comment.Commands.Create;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDto>().ReverseMap();

        CreateMap<CommentAttachment, CommentAttachmentDto>()
            .ForMember(dest => dest.DownloadUrl, opt => opt.Ignore());

        CreateMap<CreateCommentRequest, CreateCommentCommand>()
            .ForMember(dest => dest.Attachments, opt => opt.MapFrom<AttachmentResolver>());

    }
}
