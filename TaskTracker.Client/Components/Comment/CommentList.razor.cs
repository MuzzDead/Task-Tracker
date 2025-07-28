using Microsoft.AspNetCore.Components;
using TaskTracker.Client.DTOs.Comment;

namespace TaskTracker.Client.Components.Comment;

public partial class CommentList : ComponentBase
{
    [Parameter] public List<CommentDto> Comments { get; set; } = new();
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public EventCallback<(Guid commentId, string newContent)> OnCommentEdit { get; set; }
    [Parameter] public EventCallback<Guid> OnCommentDelete { get; set; }

    private async Task HandleCommentEdit(Guid commentId, string newContent)
    {
        await OnCommentEdit.InvokeAsync((commentId, newContent));
    }

    private async Task HandleCommentDelete(Guid commentId)
    {
        await OnCommentDelete.InvokeAsync(commentId);
    }
}
