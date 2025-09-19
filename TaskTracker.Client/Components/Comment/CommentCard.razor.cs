using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TaskTracker.Client.DTOs.Comment;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Components.Comment;

public partial class CommentCard : ComponentBase
{
    [Parameter] public CommentDto Comment { get; set; } = default!;
    [Parameter] public EventCallback<string> OnEdit { get; set; }
    [Parameter] public EventCallback OnDelete { get; set; }
    [Inject]
    public IUserService UserService { get; set; } = default!;

    private TextArea EditTextAreaRef;
    private bool IsEditing = false;
    private bool IsSaving = false;
    private string EditContent = string.Empty;
    private string? UserAvatar;

    protected override async Task OnInitializedAsync()
    {
        EditContent = Comment.Text;
        await LoadAvatar();
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

    private async Task LoadAvatar()
    {
        string? avatarResponse = null;

        try
        {
            avatarResponse = await UserService.GetAvatarUrlAsync(Comment.UserId);

            if (!string.IsNullOrEmpty(avatarResponse))
            {
                var jsonDoc = System.Text.Json.JsonDocument.Parse(avatarResponse);
                if (jsonDoc.RootElement.TryGetProperty("avatarUrl", out var avatarUrlElement))
                    avatarResponse = avatarUrlElement.GetString();
            }

            if (!string.IsNullOrEmpty(avatarResponse) && avatarResponse.Contains("sig="))
            {
                var separator = avatarResponse.Contains('?') ? '&' : '?';
                avatarResponse = $"{avatarResponse}{separator}t={DateTime.UtcNow.Ticks}";
            }

            UserAvatar = string.IsNullOrEmpty(avatarResponse)
                ? GeneratePlaceholderAvatar()
                : avatarResponse;
        }
        catch
        {
            UserAvatar = GeneratePlaceholderAvatar();
        }

        StateHasChanged();
    }

    private string GeneratePlaceholderAvatar()
    {
        return $"https://api.dicebear.com/7.x/identicon/svg?seed={Comment?.CreatedBy ?? "user"}&t={DateTime.UtcNow.Ticks}";
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
