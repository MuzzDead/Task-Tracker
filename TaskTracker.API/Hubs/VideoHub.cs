using Microsoft.AspNetCore.SignalR;

namespace TaskTracker.API.Hubs;

public class VideoHub : Hub
{
    public async Task JoinConference(string boardId, string userName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, boardId);
        await Clients.OthersInGroup(boardId).SendAsync("UserJoined", Context.ConnectionId, userName);
    }

    public async Task LeaveConference(string boardId)
    {
        await Clients.OthersInGroup(boardId).SendAsync("UserLeft", Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId);
    }

    // Виправлено: використовуємо ConnectionId замість User
    public async Task SendOffer(string boardId, string targetUserId, string offer)
    {
        await Clients.Client(targetUserId).SendAsync("ReceiveOffer", Context.ConnectionId, offer);
    }

    public async Task SendAnswer(string boardId, string targetUserId, string answer)
    {
        await Clients.Client(targetUserId).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
    }

    public async Task SendIceCandidate(string boardId, string targetUserId, string candidate)
    {
        await Clients.Client(targetUserId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
    }

    public async Task UpdateMediaStatus(string boardId, bool cameraEnabled, bool micEnabled)
    {
        await Clients.OthersInGroup(boardId).SendAsync("UserMediaStatusChanged",
            Context.ConnectionId, cameraEnabled, micEnabled);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
