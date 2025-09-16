using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskTracker.Application.Archice;
using TaskTracker.Application.DTOs;
using TaskTracker.Application.Storage;

namespace TaskTracker.Functions;

public class ArchiveBoardFunction
{
    private readonly ILogger<ArchiveBoardFunction> _logger;
    private readonly IBlobService _blobService;
    private readonly ICosmosDbService _cosmosService;

    public ArchiveBoardFunction(
        ILogger<ArchiveBoardFunction> logger,
        IBlobService blobService,
        ICosmosDbService cosmosService)
    {
        _logger = logger;
        _blobService = blobService;
        _cosmosService = cosmosService;
    }

    [Function(nameof(ArchiveBoardFunction))]
    public async Task Run(
        [ServiceBusTrigger("archived-boards-queue", Connection = "ServiceBusConnection")]
        string message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation($"Processing message: {message}");

        var archivationJob = new ArchivationJob();

        try
        {
            var board = JsonSerializer.Deserialize<BoardDto>(message);
            if (board == null)
                throw new InvalidOperationException("Failed to deserialize BoardDto");

            archivationJob.BoardId = board.Id;

            var blobUrl = await _blobService.UploadBoardJsonAsync(board);

            archivationJob.Status = "Success";
            archivationJob.BlobUrl = blobUrl;

            _logger.LogInformation($"Board {board.Id} archived to: {blobUrl}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing board");
            archivationJob.Status = "Failed";
            archivationJob.ErrorMessage = ex.Message;
        }
        finally
        {
            await _cosmosService.SaveArchivationJobAsync(archivationJob);
        }
    }
}