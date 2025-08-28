namespace TaskTracker.Domain.Options;

public class BlobStorageOptions
{
    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
    public string AccountName { get; set; }
    public string AccountKey { get; set; }
}