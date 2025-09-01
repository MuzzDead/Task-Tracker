using TaskTracker.Application.DTOs.OpenAi;

namespace TaskTracker.Application.OpenAi;

public interface IChatService
{
    Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken = default);
}
