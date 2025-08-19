using Blazored.LocalStorage;
using System.Text.Json;
using TaskTracker.Client.DTOs.Auth;
using TaskTracker.Client.DTOs.User;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Services;

public class AuthStateService : IAuthStateService
{
    private readonly ILocalStorageService _localStorage;
    private readonly IAuthService _authService;

    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";
    private const string UserKey = "current_user";
    private const string TokenExpiresAtKey = "token_expires_at";

    public AuthStateService(
        ILocalStorageService localStorage,
        IAuthService authService)
    {
        _localStorage = localStorage;
        _authService = authService;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public UserDto? CurrentUser { get; private set; }
    public DateTime? TokenExpiresAt { get; private set; }

    public event Action<bool> AuthStateChanged;

    private bool IsTokenExpired()
    {
        if (TokenExpiresAt == null) return true;
        return DateTime.UtcNow >= TokenExpiresAt.Value.AddMinutes(-1);
    }

    public async Task ClearAuthDataAsync()
    {
        AccessToken = null;
        RefreshToken = null;
        CurrentUser = null;
        TokenExpiresAt = null;

        await _localStorage.RemoveItemAsync(AccessTokenKey);
        await _localStorage.RemoveItemAsync(RefreshTokenKey);
        await _localStorage.RemoveItemAsync(UserKey);
        await _localStorage.RemoveItemAsync(TokenExpiresAtKey);

        AuthStateChanged?.Invoke(false);
    }

    public async Task InitializeAsync()
    {
        try
        {
            AccessToken = await _localStorage.GetItemAsync<string>(AccessTokenKey);
            RefreshToken = await _localStorage.GetItemAsync<string>(RefreshTokenKey);
            TokenExpiresAt = await _localStorage.GetItemAsync<DateTime?>(TokenExpiresAtKey);

            var userJson = await _localStorage.GetItemAsStringAsync(UserKey);
            if (!string.IsNullOrEmpty(userJson))
            {
                CurrentUser = JsonSerializer.Deserialize<UserDto>(userJson);
            }

            if (IsTokenExpired() && !string.IsNullOrEmpty(RefreshToken))
            {
                Console.Error.WriteLine("Access token expired, attempting refresh");
                await RefreshTokenAsync();
            }

            AuthStateChanged?.Invoke(true);
        }
        catch
        {
            await ClearAuthDataAsync();
        }
    }

    public async Task SetAuthDataAsync(AuthResponse authResponse)
    {
        AccessToken = authResponse.AccessToken;
        RefreshToken = authResponse.RefreshToken;
        CurrentUser = authResponse.User;
        TokenExpiresAt = authResponse.ExpiresAt;

        await _localStorage.SetItemAsync(AccessTokenKey, AccessToken);
        await _localStorage.SetItemAsync(RefreshTokenKey, RefreshToken);
        await _localStorage.SetItemAsStringAsync(UserKey, JsonSerializer.Serialize(CurrentUser));
        await _localStorage.SetItemAsync(TokenExpiresAtKey, TokenExpiresAt);

        AuthStateChanged?.Invoke(true);
    }


    public async Task RefreshTokenAsync()
    {
        if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrEmpty(RefreshToken))
        {
            await ClearAuthDataAsync();
            return;
        }

        try
        {
            var request = new RefreshTokenRequest
            {
                AccessToken = AccessToken,
                RefreshToken = RefreshToken
            };

            var authResponse = await _authService.RefreshTokenAsync(request);
            await SetAuthDataAsync(authResponse);
        }
        catch
        {
            await ClearAuthDataAsync();
        }
    }
}
