using Refit;
using System.Net;
using TaskTracker.Client.DTOs.Auth;

namespace TaskTracker.Client.Pages.Auth;

public partial class Login
{
    private LoginUserDto model = new();
    private bool isLoading = false;

    private async Task HandleValidSubmit()
    {
        isLoading = true;

        try
        {
            var response = await AuthService.LoginAsync(model);

            await AuthStateService.SetAuthDataAsync(response);
            MessageService.Success("Login successful!");

            await Task.Delay(500);

            Navigation.NavigateTo("/");
        }
        catch (ApiException ex)
        {
            var msg = ex.StatusCode switch
            {
                HttpStatusCode.Unauthorized => "Invalid email or password.",
                HttpStatusCode.BadRequest => "Invalid input data.",
                _ => $"Login error: {ex.Content}"
            };

            MessageService.Error(msg);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex}");
            MessageService.Error("Unknown error. Please try again later.");
        }
        finally
        {
            isLoading = false;
        }
    }

    private void HandleInvalidSubmit()
    {
        MessageService.Warning("Please check your input data.");
    }

    private void NavigateToRegister()
    {
        Navigation.NavigateTo("/register");
    }
}
