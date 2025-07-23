namespace TaskTracker.Client.Services.Interfaces;

public interface IPasswordHashingService
{
    string HashPassword(string password);
}
