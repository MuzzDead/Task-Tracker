using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.DTOs;

public class MemberDto
{
    public Guid BoardRoleId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole UserRole { get; set; }
}