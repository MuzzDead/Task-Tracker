namespace TaskTracker.Domain.Options;

public class BlobStorageOptions
{
    public const string SectionName = "Blob";
    public string Container { get; set; } = string.Empty;
}