using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Refit;
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
    private string? currentAvatarUrl;
    private bool isAvatarUploading = false;

    private UpdateUserDto model = new();
    private UserDto? originalUser;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserProfile();
        await LoadAvatar();
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

    private async Task LoadAvatar()
    {
        if (originalUser?.AvatarId == null)
        {
            currentAvatarUrl = GeneratePlaceholderAvatar();
            return;
        }

        try
        {
            var avatarStream = await UserService.GetAvatarAsync(originalUser.Id);
            using var memoryStream = new MemoryStream();
            await avatarStream.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            currentAvatarUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
        }
        catch
        {
            currentAvatarUrl = GeneratePlaceholderAvatar();
        }
        finally
        {
            StateHasChanged();
        }
    }

    private async Task OnAvatarInputChange(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file == null) return;

        if (!ValidateAvatarFile(file)) return;

        await UploadAvatar(file);
    }

    private async Task UploadAvatar(IBrowserFile file)
    {
        try
        {
            isAvatarUploading = true;
            StateHasChanged();

            using var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024); // 5MB max
            var streamPart = new StreamPart(stream, file.Name, file.ContentType);

            var updatedUser = await UserService.UploadAvatarAsync(originalUser!.Id, streamPart);

            originalUser = updatedUser;

            var authResponse = new AuthResponse
            {
                AccessToken = AuthStateService.AccessToken!,
                RefreshToken = AuthStateService.RefreshToken!,
                User = updatedUser,
                ExpiresAt = AuthStateService.TokenExpiresAt ?? DateTime.UtcNow.AddMinutes(30)
            };
            await AuthStateService.SetAuthDataAsync(authResponse);

            await LoadAvatar();
            MessageService.Success("Avatar updated successfully!");
        }
        catch (Exception ex)
        {
            MessageService.Error("Failed to upload avatar");
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            isAvatarUploading = false;
            StateHasChanged();
        }
    }


    protected async Task RemoveAvatar()
    {
        try
        {
            isAvatarUploading = true;
            StateHasChanged();

            var updatedUser = await UserService.DeleteAvatarAsync(originalUser!.Id);

            originalUser = updatedUser;
            currentAvatarUrl = GeneratePlaceholderAvatar();

            var authResponse = new AuthResponse
            {
                AccessToken = AuthStateService.AccessToken!,
                User = updatedUser
            };
            await AuthStateService.SetAuthDataAsync(authResponse);

            MessageService.Success("Avatar removed successfully!");
        }
        catch (Exception ex)
        {
            MessageService.Error("Failed to remove avatar");
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            isAvatarUploading = false;
            StateHasChanged();
        }
    }

    private string GeneratePlaceholderAvatar()
    {
        return $"https://api.dicebear.com/7.x/identicon/svg?seed={originalUser?.Username ?? "user"}";
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

    private bool ValidateAvatarFile(IBrowserFile file)
    {
        const int maxSizeInMB = 5;
        const long maxSizeInBytes = maxSizeInMB * 1024 * 1024;

        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };

        if (!allowedTypes.Contains(file.ContentType?.ToLower()))
        {
            MessageService.Error("Only JPG, PNG and WebP files are allowed");
            return false;
        }

        if (file.Size > maxSizeInBytes)
        {
            MessageService.Error($"File size must be less than {maxSizeInMB}MB");
            return false;
        }

        return true;
    }
}