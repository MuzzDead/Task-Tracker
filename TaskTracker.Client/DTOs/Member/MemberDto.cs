namespace TaskTracker.Client.DTOs.Member;

public class MemberDto
{
    public Guid BoardRoleId { get; set; }
    public Guid UserId { get; set; }
    public Guid BoardId { get; set; }
    public UserRole UserRole { get; set; }

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
