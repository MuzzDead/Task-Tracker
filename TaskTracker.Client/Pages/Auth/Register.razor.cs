﻿using AntDesign;
using Microsoft.AspNetCore.Components;
using Refit;
using System.Net;
using TaskTracker.Client.DTOs.Auth;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.Auth;

public partial class Register
{
    private RegisterUserDto model = new();
    private bool isLoading = false;

    [Inject] private IPasswordHashingService PasswordHashingService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private IAuthStateService AuthStateService { get; set; } = default!;
    [Inject] private IMessageService MessageService { get; set; } = default!;

    private async Task HandleValidSubmit()
    {
        isLoading = true;
        try
        {
            var registerRequest = new RegisterUserDto
            {
                Email = model.Email,
                Username = model.Username,
                Password = PasswordHashingService.HashPassword(model.Password)
            };

            var response = await AuthService.RegisterAsync(registerRequest);

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