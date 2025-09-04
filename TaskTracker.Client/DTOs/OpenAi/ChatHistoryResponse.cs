namespace TaskTracker.Client.DTOs.OpenAi;

public sealed record ChatHistoryResponse
{
    public required string Role { get; init; }
    public required string Content { get; init; }
    public DateTime Timestamp { get; init; }
}
