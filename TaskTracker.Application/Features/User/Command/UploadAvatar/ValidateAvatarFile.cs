using Microsoft.AspNetCore.Http;
using TaskTracker.Application.Exceptions;

namespace TaskTracker.Application.Features.User.Command.UploadAvatar;

public static class ValidateAvatarFile
{
    private static readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private static readonly string[] _allowedContentTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

    public static void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new BadRequestException("Avatar file is required");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            throw new BadRequestException($"File size must not exceed {MaxFileSizeBytes / (1024 * 1024)}MB");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            throw new BadRequestException($"File extension {extension} is not allowed. Allowed extensions: {string.Join(", ", _allowedExtensions)}");
        }

        if (!_allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            throw new BadRequestException($"Content type {file.ContentType} is not allowed");
        }
    }
}
