using Microsoft.AspNetCore.Http;

namespace TaskTracker.Application.DTOs.Attach;

public class CreateCommentRequest
{
    public string Text { get; set; } = string.Empty;
    public Guid CardId { get; set; }
    public Guid UserId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public IFormFileCollection? Files { get; set; }
}
