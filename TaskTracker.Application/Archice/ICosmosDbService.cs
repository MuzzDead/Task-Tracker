using TaskTracker.Application.DTOs;

namespace TaskTracker.Application.Archice;

public interface ICosmosDbService
{
    Task SaveArchivationJobAsync(ArchivationJob job);
}