using Refit;
using TaskTracker.Client.DTOs.OpenAi;

namespace TaskTracker.Client.Services.Interfaces;

public interface IAssistantService
{
    [Post("/assistant")]
    Task<ChatResponse> AskAsync([Body] ChatRequest request);

    [Get("/assistant/history/{sessionId}")]
    Task<ChatHistoryApiResponse> GetHistoryAsync(string sessionId);
}
