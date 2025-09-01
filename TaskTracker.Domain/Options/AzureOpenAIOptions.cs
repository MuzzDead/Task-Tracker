using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Domain.Options;

public class AzureOpenAIOptions
{
    public const string SectionName = "AzureOpenAI";

    [Required, Url]
    public string Endpoint { get; set; } = string.Empty;

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string DeploymentName { get; set; } = string.Empty;
}
