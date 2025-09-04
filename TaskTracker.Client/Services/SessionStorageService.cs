using Microsoft.JSInterop;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Services;

public class SessionStorageService : ISessionStorageService
{
    private const string SESSION_KEY = "ai_chat_session_id";
    private readonly IJSRuntime _jsRuntime;

    public SessionStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string?> GetSessionIdAsync()
    {
        try
        {
            var value = await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", SESSION_KEY);
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting session ID from storage: {ex.Message}");
            return null;
        }
    }

    public async Task SetSessionIdAsync(string? sessionId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                await ClearSessionIdAsync();
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", SESSION_KEY, sessionId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting session ID in storage: {ex.Message}");
        }
    }

    public async Task ClearSessionIdAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", SESSION_KEY);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing session ID from storage: {ex.Message}");
        }
    }

    public async Task<bool> HasSessionIdAsync()
    {
        try
        {
            var sessionId = await GetSessionIdAsync();
            return !string.IsNullOrWhiteSpace(sessionId);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
