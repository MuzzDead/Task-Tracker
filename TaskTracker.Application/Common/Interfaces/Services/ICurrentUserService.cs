namespace TaskTracker.Application.Common.Interfaces.Services;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
    string GetCurrentUserEmail();
    string GetCurrentUsername();
    bool IsAuthenticated();
}
