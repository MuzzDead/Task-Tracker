using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.Entities;

public class CommentAttachment : BaseAuditableEntity
{
    public Guid CommentId { get; set; }
    public Guid BlobId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
}
