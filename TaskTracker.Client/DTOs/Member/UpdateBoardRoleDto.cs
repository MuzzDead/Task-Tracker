using System.Text.Json.Serialization;

namespace TaskTracker.Client.DTOs.Member;

public class UpdateBoardRoleDto
{
    public Guid Id { get; set; }

    [JsonConverter(typeof(JsonNumberEnumConverter<UserRole>))]
    public UserRole Role { get; set; }
}
