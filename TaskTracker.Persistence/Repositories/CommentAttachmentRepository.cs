using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Common.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Persistence.Repositories.Base;

namespace TaskTracker.Persistence.Repositories;

public class CommentAttachmentRepository : BaseRepository<CommentAttachment, Guid>, ICommentAttachmentRepository
{
    public CommentAttachmentRepository(ApplicationDbContext context) : base(context) { }

    public async Task DeleteByBlobIdAsync(Guid blobId)
    {
        var attachment = await GetByBlobIdAsync(blobId);
        if (attachment != null)
        {
            _context.CommentAttachments.Remove(attachment);
        }
    }

    public async Task DeleteByCommentIdAsync(Guid commentId)
    {
        var attachments = await GetByCommentIdAsync(commentId);
        _context.CommentAttachments.RemoveRange(attachments);
    }

    public async Task<CommentAttachment?> GetByBlobIdAsync(Guid blobId)
    {
        return await _context.CommentAttachments
            .FirstOrDefaultAsync(a => a.BlobId == blobId);
    }

    public async Task<IEnumerable<CommentAttachment>> GetByCommentIdAsync(Guid commentId)
    {
        return await _context.CommentAttachments
            .Where(a => a.CommentId == commentId)
            .ToListAsync();
    }
}
