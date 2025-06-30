using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracker.Domain.Entities;

public class TaskAttachment
{
    public string FileName { get; private set; } = string.Empty;
    public string OriginalFileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string FilePath { get; private set; } = string.Empty;

    public Guid TaskId { get; private set; }
    public Guid UploadedById { get; private set; }

    public virtual TaskItem Task { get; private set; } = null!;
    public virtual User UploadedBy { get; private set; } = null!;
}
