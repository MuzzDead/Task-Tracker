using TaskTracker.Application.Common.Interfaces.Base;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Common.Interfaces;

public interface ICommentAttachmentRepository : IRepository<CommentAttachment, Guid>
{
    Task<IEnumerable<CommentAttachment>> GetByCommentIdAsync(Guid commentId);
    Task<CommentAttachment?> GetByBlobIdAsync(Guid blobId);
    Task DeleteByBlobIdAsync(Guid blobId);
    Task DeleteByCommentIdAsync(Guid commentId);
}
