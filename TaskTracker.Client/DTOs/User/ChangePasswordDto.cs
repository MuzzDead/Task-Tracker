namespace TaskTracker.Client.DTOs.User;

public class ChangePasswordDto
{
    public Guid Id { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}
