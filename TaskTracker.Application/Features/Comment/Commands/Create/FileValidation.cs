namespace TaskTracker.Application.Features.Comment.Commands.Create;

public static class FileValidation
{
    public static readonly string[] AllowedContentTypes =
    {
    "image/jpeg", "image/png", "image/gif", "image/webp",
    "application/pdf",
    "text/plain",
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    };

    public const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    public const int MaxFilesPerComment = 5;

    public static bool IsValidContentType(string contentType)
        => AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);

    public static bool IsValidSize(long size)
        => size > 0 && size <= MaxFileSize;
}