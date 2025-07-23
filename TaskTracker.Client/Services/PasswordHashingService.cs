using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Services;

public class PasswordHashingService : IPasswordHashingService
{
    private const int WorkFactor = 8;

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty");

        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }
}
