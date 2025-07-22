using Blazored.LocalStorage;
using System.Text.Json;
using TaskTracker.Client.DTOs.Auth;
using TaskTracker.Client.DTOs.User;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Services;

public class AuthStateService : IAuthStateService
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private const string TokenKey = "auth_token";
    private const string UserKey = "current_user";

    public AuthStateService(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
    public string? Token { get; private set; }
    public UserDto? CurrentUser { get; private set; }

    public event Action<bool> AuthStateChanged;

    public async Task ClearAuthDataAsync()
    {
        Token = null;
        CurrentUser = null;

        await _localStorage.RemoveItemAsync(TokenKey);
        await _localStorage.RemoveItemAsync(UserKey);

        _httpClient.DefaultRequestHeaders.Authorization = null;

        AuthStateChanged?.Invoke(false);
    }

    public async Task InitializeAsync()
    {
        try
        {
            Token = await _localStorage.GetItemAsStringAsync(TokenKey);
            var userJson = await _localStorage.GetItemAsStringAsync(UserKey);

            if (!string.IsNullOrEmpty(Token) && !string.IsNullOrEmpty(userJson))
            {
                CurrentUser = JsonSerializer.Deserialize<UserDto>(userJson);
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            }
        }
        catch
        {
            await ClearAuthDataAsync();
        }
    }

    public async Task SetAuthDataAsync(AuthResponse authResponse)
    {
        Token = authResponse.Token;
        CurrentUser = authResponse.User;

        await _localStorage.SetItemAsync(TokenKey, authResponse.Token);
        await _localStorage.SetItemAsStringAsync(UserKey, JsonSerializer.Serialize(authResponse.User));

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse.Token);

        AuthStateChanged?.Invoke(true);
    }
}
