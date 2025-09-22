using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskTracker.Application.Archice;
using TaskTracker.Application.Common.Interfaces.UnitOfWork;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Infrastructure.BackgroundServices;

public class ArchiveBoardsJob : IArchiveBoardsJob
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IServiceBusService _serviceBusService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ArchiveBoardsJob> _logger;

    public ArchiveBoardsJob(
        IUnitOfWorkFactory unitOfWorkFactory,
        IServiceBusService serviceBusService,
        IConfiguration configuration,
        ILogger<ArchiveBoardsJob> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _serviceBusService = serviceBusService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task ProcessArchivedBoards()
    {
        try
        {
            using var uow = _unitOfWorkFactory.CreateUnitOfWork();

            var archivedBoards = await uow.Boards.GetArchivedBoardsAsync();
            var boardsToProcess = archivedBoards
                .Select(b => new BoardDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    CreatedAt = b.CreatedAt,
                    CreatedBy = b.CreatedBy,
                    IsArchived = b.IsArchived,
                    ArchivedAt = b.ArchivedAt
                })
                .ToList();

            var queueName = _configuration["ServiceBus:QueueName"];

            foreach (var board in boardsToProcess)
            {
                await _serviceBusService.SendMessageAsync(board, queueName);
                _logger.LogInformation($"Board {board.Id} sent to Service Bus");
            }

            _logger.LogInformation($"Processed {boardsToProcess.Count} archived boards");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing archived boards");
            throw;
        }
    }
}
