using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TaskTracker.Client.DTOs.Comment;

namespace TaskTracker.Client.Components.Comment;

public partial class CommentCard : ComponentBase
{
    [Parameter] public CommentDto Comment { get; set; } = default!;
    [Parameter] public EventCallback<string> OnEdit { get; set; }
    [Parameter] public EventCallback OnDelete { get; set; }

    private TextArea EditTextAreaRef;
    private bool IsEditing = false;
    private bool IsSaving = false;
    private string EditContent = string.Empty;

    protected override void OnInitialized()
    {
        EditContent = Comment.Text;
    }

    private void StartEdit()
    {
        IsEditing = true;
        EditContent = Comment.Text;
    }

    private void CancelEdit()
    {
        IsEditing = false;
        EditContent = Comment.Text;
    }

    private async Task SaveEdit()
    {
        if (string.IsNullOrWhiteSpace(EditContent) || EditContent == Comment.Text)
        {
            CancelEdit();
            return;
        }

        IsSaving = true;
        try
        {
            await OnEdit.InvokeAsync(EditContent);
            Comment.Text = EditContent;
            IsEditing = false;
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task DeleteComment()
    {
        await OnDelete.InvokeAsync();
    }

    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && (e.CtrlKey || e.MetaKey))
        {
            _ = SaveEdit();
        }
    }

    private string FormatDate(DateTimeOffset date)
    {
        return date.ToLocalTime().ToString("g");
    }

    private string FormatContent(string content)
    {
        return System.Net.WebUtility.HtmlEncode(content).Replace("\n", "<br />");
    }

    private string FormatFileSize(long size)
    {
        if (size < 1024) return $"{size} B";
        if (size < 1024 * 1024) return $"{size / 1024.0:F1} KB";
        return $"{size / 1024.0 / 1024.0:F1} MB";
    }

    private string GetFileIcon(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLower();
        return ext switch
        {
            ".pdf" => "file-pdf",
            ".doc" or ".docx" => "file-word",
            ".xls" or ".xlsx" => "file-excel",
            ".png" or ".jpg" or ".jpeg" or ".gif" => "file-image",
            _ => "file"
        };
    }
}
