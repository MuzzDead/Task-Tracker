namespace TaskTracker.Client.DTOs.Member;

public class BoardRoleDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BoardId { get; set; }
    public UserRole UserRole { get; set; }
}
