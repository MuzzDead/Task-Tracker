namespace TaskTracker.Application.Storage;

public interface IBlobService
{
    Task<Guid> UploadAsync(Stream content, string contentType);
    string GenerateSasToken(Guid blobId, int expiresInMinutes = 5);
    Task DeleteAsync(Guid blobId);
}