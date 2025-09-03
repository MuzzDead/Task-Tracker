using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TaskTracker.Client.DTOs.OpenAi;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.Assistant;

public partial class Assistant : ComponentBase
{
    [Inject] private IAssistantService AssistantService { get; set; } = null!;
    [Inject] private IMessageService MessageService { get; set; } = null!;
    [Inject] private ISessionStorageService SessionStorageService { get; set; } = null!;

    private string UserInput { get; set; } = string.Empty;
    private List<ChatMessage> Messages { get; set; } = new();
    private bool IsLoading { get; set; } = false;
    private string? CurrentSessionId { get; set; }
    private bool IsLoadingHistory { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadInitialHistory();
        await base.OnInitializedAsync();
    }

    private async Task LoadInitialHistory()
    {
        IsLoadingHistory = true;
        StateHasChanged();

        try
        {
            CurrentSessionId = await SessionStorageService.GetSessionIdAsync();

            if (!string.IsNullOrWhiteSpace(CurrentSessionId))
            {
                await LoadHistoryForSession(CurrentSessionId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load initial history: {ex.Message}");
        }
        finally
        {
            IsLoadingHistory = false;
            StateHasChanged();
        }
    }

    private async Task LoadHistoryForSession(string sessionId)
    {
        try
        {
            var history = await AssistantService.GetHistoryAsync(sessionId);
            Messages.Clear();

            foreach (var msg in history.Messages)
            {
                Messages.Add(new ChatMessage
                {
                    Text = msg.Content,
                    IsUser = msg.Role.Equals("User", StringComparison.OrdinalIgnoreCase),
                    IsLoading = false
                });
            }
        }
        catch (Exception)
        {
            Messages.Clear();
            CurrentSessionId = null;
        }
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(UserInput) || IsLoading)
            return;

        var userText = UserInput.Trim();
        UserInput = string.Empty;
        IsLoading = true;

        Messages.Add(new ChatMessage
        {
            Text = userText,
            IsUser = true,
            IsLoading = false
        });

        var loadingMessage = new ChatMessage
        {
            Text = string.Empty,
            IsUser = false,
            IsLoading = true
        };
        Messages.Add(loadingMessage);
        StateHasChanged();

        try
        {
            var request = new ChatRequest
            {
                Message = userText,
                SessionId = CurrentSessionId
            };

            var response = await AssistantService.AskAsync(request);
            CurrentSessionId = response.SessionId;

            Messages.Remove(loadingMessage);
            Messages.Add(new ChatMessage
            {
                Text = response.Message,
                IsUser = false,
                IsLoading = false
            });

            await SessionStorageService.SetSessionIdAsync(CurrentSessionId);
        }
        catch (Exception ex)
        {
            Messages.Remove(loadingMessage);
            Messages.Add(new ChatMessage
            {
                Text = "An error occurred. Please try again.",
                IsUser = false,
                IsLoading = false,
                IsError = true
            });
            MessageService.Error($"Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task ClearChat()
    {
        Messages.Clear();
        CurrentSessionId = null;
        await SessionStorageService.ClearSessionIdAsync();
        StateHasChanged();
    }

    private async Task HandleEnter(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !IsLoading)
        {
            await SendMessage();
        }
    }

    public class ChatMessage
    {
        public string Text { get; set; } = string.Empty;
        public bool IsUser { get; set; }
        public bool IsLoading { get; set; }
        public bool IsError { get; set; }
    }
}