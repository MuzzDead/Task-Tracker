using System.Text.Json.Serialization;

namespace TaskTracker.Client.DTOs.Member;

public class CreateBoardRoleDto
{
    public Guid UserId { get; set; }
    public Guid BoardId { get; set; }

    [JsonConverter(typeof(JsonNumberEnumConverter<UserRole>))]
    public UserRole Role { get; set; }
}
