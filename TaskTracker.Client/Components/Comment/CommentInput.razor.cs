using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace TaskTracker.Client.Components.Comment
{
    public partial class CommentInput : ComponentBase
    {
        [Parameter] public EventCallback<CommentSubmissionData> OnCommentSubmit { get; set; }
        [Parameter] public bool IsSubmitting { get; set; }

        private string CommentText = string.Empty;
        private List<IBrowserFile> SelectedFiles { get; set; } = new();
        private List<string> ValidationErrors = new();
        private CommentSubmissionData submissionData = new();
        private ElementReference? fileInput;


        private readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".txt", ".jpg", ".jpeg", ".png"
        };

        private readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "text/plain",
            "image/jpeg",
            "image/jpg",
            "image/png"
        };

        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
        private const int MaxFiles = 5;

        private async Task OnFilesSelected(InputFileChangeEventArgs e)
        {
            SelectedFiles = e.GetMultipleFiles().ToList();
            StateHasChanged();
        }

        private async Task SubmitComment()
        {
            if (string.IsNullOrWhiteSpace(CommentText) && !SelectedFiles.Any())
                return;

            ClearValidationErrors();

            if (!ValidateFiles())
                return;

            var comment = CommentText.Trim();
            var files = SelectedFiles.ToList();

            CommentText = string.Empty;
            SelectedFiles.Clear();

            var submissionData = new CommentSubmissionData
            {
                Text = comment,
                Files = files
            };

            await OnCommentSubmit.InvokeAsync(submissionData);

            submissionData = new CommentSubmissionData();
        }

        private async Task HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && e.CtrlKey && (!string.IsNullOrWhiteSpace(CommentText) || SelectedFiles.Any()))
            {
                await SubmitComment();
            }
        }

        private void RemoveFile(IBrowserFile file)
        {
            SelectedFiles.Remove(file);
            ClearValidationErrors();
            StateHasChanged();
        }

        private bool ValidateFiles()
        {
            var isValid = true;

            if (SelectedFiles.Count > MaxFiles)
            {
                AddValidationError($"Too many files selected. Maximum {MaxFiles} files allowed");
                isValid = false;
            }

            foreach (var file in SelectedFiles)
            {
                if (file.Size > MaxFileSize)
                {
                    AddValidationError($"File '{file.Name}' is too large. Maximum size is {FormatFileSize(MaxFileSize)}");
                    isValid = false;
                }

                var extension = System.IO.Path.GetExtension(file.Name);
                if (!AllowedExtensions.Contains(extension))
                {
                    AddValidationError($"File '{file.Name}' has unsupported type '{extension}'");
                    isValid = false;
                }
            }

            return isValid;
        }

        private void AddValidationError(string error)
        {
            if (!ValidationErrors.Contains(error))
            {
                ValidationErrors.Add(error);
            }
        }

        private void ClearValidationErrors()
        {
            ValidationErrors.Clear();
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private string GetFileIcon(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "file-pdf",
                ".txt" => "file-text",
                ".jpg" or ".jpeg" or ".png" => "file-image",
                _ => "file"
            };
        }
    }

    public class CommentSubmissionData
    {
        public string Text { get; set; } = string.Empty;
        public List<IBrowserFile> Files { get; set; } = new();
    }
}