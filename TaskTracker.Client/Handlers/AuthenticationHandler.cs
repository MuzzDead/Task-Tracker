using System.Net;
using System.Net.Http.Headers;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Handlers;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly IAuthStateService _authStateService;
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);
    private readonly SemaphoreSlim _initSemaphore = new(1, 1);
    private bool _isInitialized = false;

    public AuthenticationHandler(IAuthStateService authStateService)
    {
        _authStateService = authStateService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync();

        if (!IsAuthEndpoint(request.RequestUri))
        {
            if (ShouldRefreshToken())
            {
                await TryRefreshTokenAsync();
            }

            if (!string.IsNullOrEmpty(_authStateService.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authStateService.AccessToken);
            }
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized &&
            !IsAuthEndpoint(request.RequestUri) &&
            !string.IsNullOrEmpty(_authStateService.RefreshToken))
        {
            var refreshSuccess = await TryRefreshTokenAsync();

            if (refreshSuccess)
            {
                var clonedRequest = await CloneRequestAsync(request);
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authStateService.AccessToken);

                response.Dispose();
                response = await base.SendAsync(clonedRequest, cancellationToken);
            }
        }

        return response;
    }

    private async Task EnsureInitializedAsync()
    {
        if (_isInitialized) return;

        await _initSemaphore.WaitAsync();
        try
        {
            if (!_isInitialized)
            {
                await _authStateService.InitializeAsync();
                _isInitialized = true;
            }
        }
        finally
        {
            _initSemaphore.Release();
        }
    }

    private bool ShouldRefreshToken()
    {
        return _authStateService.TokenExpiresAt.HasValue &&
               DateTime.UtcNow >= _authStateService.TokenExpiresAt.Value.AddMinutes(-2);
    }

    private async Task<bool> TryRefreshTokenAsync()
    {
        if (!await _refreshSemaphore.WaitAsync(5000))
        {
            return false;
        }

        try
        {
            if (!ShouldRefreshToken() || string.IsNullOrEmpty(_authStateService.RefreshToken))
            {
                return !string.IsNullOrEmpty(_authStateService.AccessToken);
            }

            await _authStateService.RefreshTokenAsync();
            return !string.IsNullOrEmpty(_authStateService.AccessToken);
        }
        catch (Exception ex)
        {
            return false;
        }
        finally
        {
            _refreshSemaphore.Release();
        }
    }

    private static bool IsAuthEndpoint(Uri? requestUri)
    {
        return requestUri?.AbsolutePath.Contains("/api/auth") == true;
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage originalRequest)
    {
        var clonedRequest = new HttpRequestMessage(originalRequest.Method, originalRequest.RequestUri);

        foreach (var header in originalRequest.Headers)
        {
            clonedRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (originalRequest.Content != null)
        {
            var contentBytes = await originalRequest.Content.ReadAsByteArrayAsync();
            clonedRequest.Content = new ByteArrayContent(contentBytes);

            foreach (var header in originalRequest.Content.Headers)
            {
                clonedRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clonedRequest;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshSemaphore?.Dispose();
        }
        base.Dispose(disposing);
    }
}