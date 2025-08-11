using System.Net.Http.Headers;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Handlers;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly IAuthStateService _authStateService;

    public AuthenticationHandler(IAuthStateService authStateService)
    {
        _authStateService = authStateService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await _authStateService.InitializeAsync();

        if (!string.IsNullOrEmpty(_authStateService.Token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _authStateService.Token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}