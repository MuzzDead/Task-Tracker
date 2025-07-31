using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TaskTracker.Client.DTOs.User;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.Profile;

public partial class ChangePasswordPage : ComponentBase
{
    [Inject] protected IUserService UserService { get; set; } = default!;
    [Inject] protected IAuthStateService AuthStateService { get; set; } = default!;
    [Inject] protected IPasswordHashingService PasswordHashingService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IMessageService MessageService { get; set; } = default!;

    protected string currentPassword = string.Empty;
    protected string newPassword = string.Empty;
    protected string confirmPassword = string.Empty;

    protected bool isLoading = true;
    protected bool isSubmitting = false;
    private Guid currentUserId = Guid.Empty;

    protected override async Task OnInitializedAsync()
    {
        await InitializeComponent();
    }

    private async Task InitializeComponent()
    {
        try
        {
            isLoading = true;
            StateHasChanged();

            await AuthStateService.InitializeAsync();

            if (!AuthStateService.IsAuthenticated || AuthStateService.CurrentUser == null)
            {
                Navigation.NavigateTo("/login");
                return;
            }

            currentUserId = AuthStateService.CurrentUser.Id;
        }
        catch (Exception ex)
        {
            MessageService.Error("Failed to initialize");
            Console.WriteLine($"Error: {ex.Message}");
            Navigation.NavigateTo("/login");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task ChangePassword()
    {
        if (!IsFormValid() || isSubmitting)
            return;

        try
        {
            isSubmitting = true;
            StateHasChanged();

            var hashedCurrentPassword = PasswordHashingService.HashPassword(currentPassword);
            var hashedNewPassword = PasswordHashingService.HashPassword(newPassword);

            var changePasswordDto = new ChangePasswordDto
            {
                Id = currentUserId,
                CurrentPassword = hashedCurrentPassword,
                NewPassword = hashedNewPassword
            };

            await UserService.ChangePasswordAsync(currentUserId, changePasswordDto);

            MessageService.Success("Password changed successfully!");

            ClearForm();
            Navigation.NavigateTo("/profile");
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("Unauthorized"))
        {
            MessageService.Error("Current password is incorrect");
        }
        catch (Exception ex)
        {
            MessageService.Error("Failed to change password");
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            isSubmitting = false;
            StateHasChanged();
        }
    }

    protected void Cancel()
    {
        ClearForm();
        Navigation.NavigateTo("/profile");
    }

    protected void HandleEnterPress(KeyboardEventArgs args)
    {
        if (args.Key == "Enter" && IsFormValid() && !isSubmitting)
        {
            _ = Task.Run(ChangePassword);
        }
    }

    protected bool IsFormValid()
    {
        return !string.IsNullOrWhiteSpace(currentPassword) &&
               !string.IsNullOrWhiteSpace(newPassword) &&
               !string.IsNullOrWhiteSpace(confirmPassword) &&
               newPassword == confirmPassword &&
               newPassword.Length >= 6 &&
               currentPassword != newPassword;
    }

    private void ClearForm()
    {
        currentPassword = string.Empty;
        newPassword = string.Empty;
        confirmPassword = string.Empty;
    }
}
