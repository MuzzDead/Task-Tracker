using System.Text.Json.Serialization;

namespace TaskTracker.Client.DTOs.State;

public class UpdateStateDto
{
    public Guid CardId { get; set; }
    public string? Description { get; set; }
    public bool? IsCompleted { get; set; }

    [JsonConverter(typeof(JsonNumberEnumConverter<Priority>))]
    public Priority? Priority { get; set; }
    public DateTimeOffset? Deadline { get; set; }
    public Guid? AssigneeId { get; set; }
    public Guid? UpdatedBy { get; set; }
}
