using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using TaskTracker.Application.Storage;
using TaskTracker.Domain.Options;

namespace TaskTracker.Infrastructure.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly string _accountName;
    private readonly string _accountKey;

    public BlobService(IOptions<BlobStorageOptions> options)
    {
        _containerName = options.Value.ContainerName;
        _accountName = options.Value.AccountName;
        _accountKey = options.Value.AccountKey;
        _blobServiceClient = new BlobServiceClient(options.Value.ConnectionString);
    }

    public async Task DeleteAsync(Guid blobId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobId.ToString());
        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
    }

    public string GenerateSasToken(Guid blobId, int expiresInMinutes = 5)
    {
        var blobSasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobId.ToString(),
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiresInMinutes)
        };

        blobSasBuilder.SetPermissions(BlobSasPermissions.Read);
        var sasToken = blobSasBuilder.ToSasQueryParameters(
            new StorageSharedKeyCredential(_accountName, _accountKey)).ToString();

        var baseUrl = $"https://{_accountName}.blob.core.windows.net";
        return $"{baseUrl}/{_containerName}/{blobId}?{sasToken}";
    }
    
    public async Task<Guid> UploadAsync(Stream content, string contentType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobId = Guid.NewGuid();
        var blobClient = containerClient.GetBlobClient(blobId.ToString());

        await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });
        return blobId;
    }
}
