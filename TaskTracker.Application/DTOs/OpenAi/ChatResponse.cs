namespace TaskTracker.Application.DTOs.OpenAi;

public sealed record ChatResponse
{
    public required string Message { get; init; }
}
