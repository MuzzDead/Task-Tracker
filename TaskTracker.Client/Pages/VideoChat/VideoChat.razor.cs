using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using TaskTracker.Client.Services.Interfaces;

namespace TaskTracker.Client.Pages.VideoChat;

public partial class VideoChat : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IMessageService MessageService { get; set; } = default!;
    [Inject] private IAuthStateService AuthStateService { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;

    [Parameter] public Guid BoardId { get; set; }

    private IJSObjectReference? _module;
    private HubConnection? _hubConnection;
    private Dictionary<string, UserInfo> _connectedUsers = new();
    private bool _isConnected = false;
    private bool _isCameraEnabled = true;
    private bool _isMicrophoneEnabled = true;
    private bool _isInitializing = false;
    private string? _myUserId;

    private bool isScreenSharing = false;
    private bool isScreenShareLoading = false;

    public class UserInfo
    {
        public string Name { get; set; } = "";
        public bool CameraEnabled { get; set; } = true;
        public bool MicEnabled { get; set; } = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var apiRoot = Configuration["ApiRootUrl"]!;

        if (firstRender)
        {
            _isInitializing = true;
            StateHasChanged();

            _module = await JS.InvokeAsync<IJSObjectReference>(
                "import", "/js/videochat/videochat.js"
            );

            await _module.InvokeVoidAsync("initVideo",
                DotNetObjectReference.Create(this));

            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{apiRoot}/videoHub")
                .WithAutomaticReconnect()
                .Build();

            SetupHubHandlers();

            try
            {
                await _hubConnection.StartAsync();
                _myUserId = _hubConnection.ConnectionId;

                var username = AuthStateService.CurrentUser?.Username
               ?? "Unknown User";

                var mediaState = await _module.InvokeAsync<MediaState>("getMediaState");
                await _hubConnection.InvokeAsync("JoinConference",
                    BoardId.ToString(),
                    username,
                    mediaState.IsCameraEnabled,
                    mediaState.IsMicEnabled);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hub connection error: {ex.Message}");
            }
        }
        else if (_isConnected)
        {
            await _module.InvokeVoidAsync("connectLocalVideo");
        }
    }

    private void SetupHubHandlers()
    {
        _hubConnection.On<string, string>("UserJoined", async (userId, userName) =>
        {
            await OnUserJoined(userId, userName);
        });

        _hubConnection.On<string>("UserLeft", async (userId) =>
        {
            await OnUserLeft(userId);
        });

        _hubConnection.On<string, string>("ReceiveOffer", async (userId, offer) =>
        {
            await OnReceiveOffer(userId, offer);
        });

        _hubConnection.On<string, string>("ReceiveAnswer", async (userId, answer) =>
        {
            await OnReceiveAnswer(userId, answer);
        });

        _hubConnection.On<string, string>("ReceiveIceCandidate", async (userId, candidate) =>
        {
            await OnReceiveIceCandidate(userId, candidate);
        });

        _hubConnection.On<string, bool, bool>("UserMediaStatusChanged", async (userId, cam, mic) =>
        {
            await OnUserMediaStatusChanged(userId, cam, mic);
        });
    }

    [JSInvokable]
    public async Task OnInitialized(bool cameraEnabled, bool micEnabled)
    {
        _isCameraEnabled = cameraEnabled;
        _isMicrophoneEnabled = micEnabled;
        _isConnected = true;
        _isInitializing = false;
        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task SendOffer(string targetUserId, string offer)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("SendOffer", BoardId.ToString(), targetUserId, offer);
        }
    }

    [JSInvokable]
    public async Task SendAnswer(string targetUserId, string answer)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("SendAnswer", BoardId.ToString(), targetUserId, answer);
        }
    }

    [JSInvokable]
    public async Task SendIceCandidate(string targetUserId, string candidate)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("SendIceCandidate", BoardId.ToString(), targetUserId, candidate);
        }
    }

    [JSInvokable]
    public async Task UpdateMediaStatus(bool cameraEnabled, bool micEnabled)
    {
        if (_hubConnection != null)
        {
            await _hubConnection.InvokeAsync("UpdateMediaStatus", BoardId.ToString(), cameraEnabled, micEnabled);
        }
    }

    private async Task OnUserJoined(string userId, string userName)
    {
        if (!_connectedUsers.ContainsKey(userId))
        {
            _connectedUsers[userId] = new UserInfo { Name = userName };
            await InvokeAsync(StateHasChanged);

            await _module.InvokeVoidAsync("createPeerConnection", userId);

            if (_myUserId != null && string.Compare(_myUserId, userId) < 0)
            {
                await _module.InvokeVoidAsync("createOffer", userId);
            }
        }
    }

    private async Task OnUserLeft(string userId)
    {
        _connectedUsers.Remove(userId);
        await _module.InvokeVoidAsync("removePeer", userId);
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnReceiveOffer(string userId, string offer)
    {
        await _module.InvokeVoidAsync("handleOffer", userId, offer);
    }

    private async Task OnReceiveAnswer(string userId, string answer)
    {
        await _module.InvokeVoidAsync("handleAnswer", userId, answer);
    }

    private async Task OnReceiveIceCandidate(string userId, string candidate)
    {
        await _module.InvokeVoidAsync("handleIceCandidate", userId, candidate);
    }

    private async Task OnUserMediaStatusChanged(string userId, bool cameraEnabled, bool micEnabled)
    {
        if (_connectedUsers.ContainsKey(userId))
        {
            _connectedUsers[userId].CameraEnabled = cameraEnabled;
            _connectedUsers[userId].MicEnabled = micEnabled;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task ToggleCamera()
    {
        _isCameraEnabled = !_isCameraEnabled;
        await _module.InvokeVoidAsync("toggleCamera", _isCameraEnabled);
    }

    private async Task ToggleMicrophone()
    {
        _isMicrophoneEnabled = !_isMicrophoneEnabled;
        await _module.InvokeVoidAsync("toggleMicrophone", _isMicrophoneEnabled);
    }

    private async Task ToggleScreenShare()
    {
        isScreenShareLoading = true;
        try
        {
            if (!isScreenSharing)
            {
                await _module.InvokeVoidAsync("startScreenShare");
                isScreenSharing = true;
            }
            else
            {
                await _module.InvokeVoidAsync("stopScreenShare");
                isScreenSharing = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Screen share error: {ex.Message}");
            MessageService.Error("Failed to share screen");
        }
        finally
        {
            isScreenShareLoading = false;
            StateHasChanged();
        }
    }

    [JSInvokable]
    public async Task OnScreenShareStopped()
    {
        isScreenSharing = false;
        StateHasChanged();
        MessageService.Info("Screen sharing stopped");
    }

    private async Task LeaveConference()
    {
        if (_hubConnection != null)
        {
            try
            {
                await _hubConnection.InvokeAsync("LeaveConference", BoardId.ToString());
                await _hubConnection.StopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error leaving conference: {ex.Message}");
            }
        }
        GoBack();
    }

    private void GoBack()
    {
        Navigation.NavigateTo($"/boards/{BoardId}");
    }

    public async ValueTask DisposeAsync()
    {
        if (isScreenSharing)
        {
            await _module.InvokeVoidAsync("stopScreenShare");
        }

        if (_module != null)
        {
            try
            {
                await _module.InvokeVoidAsync("cleanup");
                await _module.DisposeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }

        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    public class MediaState
    {
        public bool IsCameraEnabled { get; set; }
        public bool IsMicEnabled { get; set; }
    }
}
