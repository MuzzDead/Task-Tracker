using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace TaskTracker.Client.Components.Comment
{
    public partial class CommentInput : ComponentBase
    {
        [Parameter] public EventCallback<string> OnCommentSubmit { get; set; }
        [Parameter] public bool IsSubmitting { get; set; }

        private string CommentText = string.Empty;
        private Input<string>? TextAreaRef;

        private async Task SubmitComment()
        {
            if (string.IsNullOrWhiteSpace(CommentText)) return;

            var comment = CommentText.Trim();
            CommentText = string.Empty;

            await OnCommentSubmit.InvokeAsync(comment);
        }

        private async Task HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && e.CtrlKey && !string.IsNullOrWhiteSpace(CommentText))
            {
                await SubmitComment();
            }
        }
    }
}