using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TaskTracker.Client.DTOs.Auth;
using TaskTracker.Client.DTOs.User;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.Profile;

public partial class EditProfileInfo : ComponentBase
{
    [Inject] protected IUserService UserService { get; set; } = default!;
    [Inject] protected IAuthStateService AuthStateService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IMessageService MessageService { get; set; } = default!;

    protected bool isLoading = true;
    protected bool isSubmitting = false;

    private UpdateUserDto model = new();
    private UserDto? originalUser;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserProfile();
    }

    private async Task LoadUserProfile()
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

            var currentUser = AuthStateService.CurrentUser!;
            originalUser = await UserService.GetByIdAsync(currentUser.Id);

            model.Username = originalUser.Username;
            model.Email = originalUser.Email;
        }
        catch (Exception ex)
        {
            MessageService.Error("Failed to load profile");
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    protected async Task SaveChanges()
    {
        if (!IsFormValid() || isSubmitting)
            return;

        try
        {
            isSubmitting = true;
            StateHasChanged();

            var updateDto = new UpdateUserDto
            {
                Id = originalUser!.Id,
                Username = model.Username.Trim(),
                Email = model.Email.Trim()
            };

            await UserService.UpdateAsync(updateDto.Id, updateDto);

            var updatedUser = new UserDto
            {
                Id = updateDto.Id,
                Username = updateDto.Username,
                Email = updateDto.Email
            };

            var authResponse = new AuthResponse
            {
                AccessToken = AuthStateService.AccessToken!,
                User = updatedUser
            };

            await AuthStateService.SetAuthDataAsync(authResponse);

            originalUser = updatedUser;

            MessageService.Success("Profile updated successfully!");
            Navigation.NavigateTo("/profile");
        }
        catch (Exception ex)
        {
            MessageService.Error("Failed to update profile");
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
        if (HasChanges())
        {
            model.Username = originalUser?.Username ?? string.Empty;
            model.Email = originalUser?.Email ?? string.Empty;
            StateHasChanged();
        }

        Navigation.NavigateTo("/profile");
    }

    protected void HandleEnterPress(KeyboardEventArgs args)
    {
        if (args.Key == "Enter" && IsFormValid() && !isSubmitting)
        {
            _ = Task.Run(SaveChanges);
        }
    }

    protected void ChangeAvatar()
    {
        MessageService.Info("TODO");
    }

    protected bool IsFormValid()
    {
        return !string.IsNullOrWhiteSpace(model.Username) &&
               !string.IsNullOrWhiteSpace(model.Email) &&
               IsValidEmail(model.Email) &&
               HasChanges();
    }

    private bool HasChanges()
    {
        if (originalUser == null) return false;

        return originalUser.Username != model.Username.Trim() ||
               originalUser.Email != model.Email.Trim();
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}