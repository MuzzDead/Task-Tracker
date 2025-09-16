using Microsoft.Azure.Cosmos;
using TaskTracker.Application.Archice;
using TaskTracker.Application.DTOs;

namespace TaskTracker.Infrastructure.Services;

public class CosmosDbService : ICosmosDbService
{
    private readonly Container _container;
    public CosmosDbService(CosmosClient cosmosClient)
    {
        var database = cosmosClient.GetDatabase("TaskTrackerDb");
        _container = database.GetContainer("ArchivationJobs");
    }

    public async Task SaveArchivationJobAsync(ArchivationJob job)
    {
        await _container.CreateItemAsync(job, new PartitionKey(job.BoardId.ToString()));
    }
}
