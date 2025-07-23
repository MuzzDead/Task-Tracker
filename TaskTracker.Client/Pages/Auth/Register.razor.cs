using Microsoft.AspNetCore.Components;
using Refit;
using System.Net;
using TaskTracker.Client.DTOs.Auth;
using TaskTracker.Client.Services;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.Auth;

public partial class Register
{
    private RegisterUserDto model = new();
    private bool isLoading = false;

    [Inject] private IPasswordHashingService PasswordHashingService { get; set; } = default!;

    private async Task HandleValidSubmit()
    {
        isLoading = true;
        try
        {
            var loginRequest = new LoginUserDto
            {
                Email = model.Email,
                Password = PasswordHashingService.HashPassword(model.Password)
            };

            var response = await AuthService.RegisterAsync(model);

            await AuthStateService.SetAuthDataAsync(response);

            MessageService.Success("Registration successful!");
            await Task.Delay(1000);

            Navigation.NavigateTo("/");
        }
        catch (ApiException ex)
        {
            await HandleApiException(ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Register error: {ex}");
            MessageService.Error("Unknown error. Please try again.");
        }
        finally
        {
            isLoading = false;
        }
    }

    private void HandleInvalidSubmit()
    {
        MessageService.Warning("Please check your input data");
    }

    private void NavigateToRegister()
    {
        Navigation.NavigateTo("/login");
    }

    private async Task HandleApiException(ApiException ex)
    {
        var message = ex.StatusCode switch
        {
            HttpStatusCode.Conflict => "A user with this email already exists.",
            HttpStatusCode.BadRequest => "Invalid input data.",
            _ => $"Error: {ex.Content}"
        };

        MessageService.Error(message);
    }
}