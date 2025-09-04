using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Client.DTOs.OpenAi;

public sealed record ChatRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public required string Message { get; init; }

    public string? SessionId { get; init; }
}
