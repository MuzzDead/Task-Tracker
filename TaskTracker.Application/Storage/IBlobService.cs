using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Storage;

public interface IBlobService
{
    Task<Guid> UploadAsync(Stream content, string contentType, string containerName);
    string GenerateSasToken(Guid blobId, string containerName, int expiresInMinutes = 5);
    Task DeleteAsync(Guid blobId, string containerName);

    Task<string> UploadBoardJsonAsync(BoardDto board, string containerName = "archived-boards");
}