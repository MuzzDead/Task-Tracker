using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using TaskTracker.Application.Storage;
using TaskTracker.Domain.Options;

namespace TaskTracker.Infrastructure.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    public BlobService(BlobServiceClient blobServiceClient, IOptions<BlobStorageOptions> options)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = options.Value.Container;
    }

    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

        Response<BlobDownloadResult> response = await blobClient.DownloadContentAsync(cancellationToken: cancellationToken);
        return new FileResponse(response.Value.Content.ToStream(), response.Value.Details.ContentType);
    }

    public async Task<Guid> UploadAsync(Stream stream, string conntentType, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        await containerClient.CreateIfNotExistsAsync();

        var fileId = Guid.NewGuid();
        BlobClient blobClient = containerClient.GetBlobClient(fileId.ToString());

        await blobClient.UploadAsync(
            stream,
            new BlobHttpHeaders { ContentType = conntentType },
            cancellationToken: cancellationToken);

        return fileId;
    }
}
