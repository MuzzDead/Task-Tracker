namespace TaskTracker.Client.DTOs.OpenAi;

public sealed record ChatResponse
{
    public required string Message { get; init; }
    public required string SessionId { get; init; }
}
