namespace TaskTracker.Client.Services.Interfaces;

public interface ISessionStorageService
{
    Task<string?> GetSessionIdAsync();
    Task SetSessionIdAsync(string? sessionId);
    Task ClearSessionIdAsync();
    Task<bool> HasSessionIdAsync();
}
