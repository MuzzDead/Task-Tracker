using AutoMapper;
using TaskTracker.Application.Features.Comment.Commands.Create;

namespace TaskTracker.Application.DTOs.Attach;

public class AttachmentResolver : IValueResolver<CreateCommentRequest, CreateCommentCommand, ICollection<AttachmentUpload>>
{
    public ICollection<AttachmentUpload> Resolve(CreateCommentRequest source, CreateCommentCommand destination,
        ICollection<AttachmentUpload> destMember, ResolutionContext context)
    {
        var attachments = new List<AttachmentUpload>();

        if (source.Files?.Any() == true)
        {
            foreach (var file in source.Files)
            {
                if (file.Length > 0)
                {
                    var stream = new MemoryStream();
                    file.CopyTo(stream);
                    stream.Position = 0;

                    attachments.Add(new AttachmentUpload
                    {
                        Content = stream,
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        Size = file.Length
                    });
                }
            }
        }

        return attachments;
    }
}