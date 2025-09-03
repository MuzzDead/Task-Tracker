namespace TaskTracker.Client.DTOs.OpenAi;

public sealed record ChatHistoryApiResponse
{
    public required string SessionId { get; init; }
    public required List<ChatHistoryResponse> Messages { get; init; }
}
