namespace TaskTracker.Client.DTOs.User;

public class UpdateUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}