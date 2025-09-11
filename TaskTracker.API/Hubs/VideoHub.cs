using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace TaskTracker.API.Hubs;

public class VideoHub : Hub
{
    private static readonly ConcurrentDictionary<string, HashSet<string>> _roomUsers = new();
    private static readonly ConcurrentDictionary<string, UserMediaStatus> _userMediaStatus = new();

    public class UserMediaStatus
    {
        public string UserName { get; set; } = "";
        public bool CameraEnabled { get; set; } = false;
        public bool MicEnabled { get; set; } = false;
    }

    public async Task JoinConference(string boardId, string userName, bool cameraEnabled, bool micEnabled)
    {
        var userId = Context.ConnectionId;

        await Groups.AddToGroupAsync(Context.ConnectionId, boardId);

        _roomUsers.AddOrUpdate(boardId,
            new HashSet<string> { userId },
            (key, existing) => { existing.Add(userId); return existing; });

        _userMediaStatus[userId] = new UserMediaStatus
        {
            UserName = userName,
            CameraEnabled = cameraEnabled,
            MicEnabled = micEnabled
        };

        var existingUsers = _roomUsers.GetValueOrDefault(boardId, new HashSet<string>())
            .Where(id => id != userId)
            .ToList();

        await Clients.OthersInGroup(boardId).SendAsync("UserJoined", userId, userName);

        foreach (var existingUserId in existingUsers)
        {
            var existingUserStatus = _userMediaStatus.GetValueOrDefault(existingUserId);
            if (existingUserStatus != null)
            {
                await Clients.Caller.SendAsync("UserJoined", existingUserId, existingUserStatus.UserName);

                if (existingUserStatus.CameraEnabled || existingUserStatus.MicEnabled)
                {
                    await Clients.Caller.SendAsync("UserMediaStatusChanged",
                        existingUserId, existingUserStatus.CameraEnabled, existingUserStatus.MicEnabled);
                }
            }
        }
    }

    public async Task LeaveConference(string boardId)
    {
        var userId = Context.ConnectionId;

        if (_roomUsers.TryGetValue(boardId, out var users))
        {
            users.Remove(userId);
            if (!users.Any())
            {
                _roomUsers.TryRemove(boardId, out _);
            }
        }
        _userMediaStatus.TryRemove(userId, out _);

        await Clients.OthersInGroup(boardId).SendAsync("UserLeft", userId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId);
    }

    public async Task SendOffer(string boardId, string targetUserId, string offer)
    {
        var roomUsers = _roomUsers.GetValueOrDefault(boardId, new HashSet<string>());
        if (roomUsers.Contains(Context.ConnectionId) && roomUsers.Contains(targetUserId))
        {
            await Clients.Client(targetUserId).SendAsync("ReceiveOffer", Context.ConnectionId, offer);
        }
    }

    public async Task SendAnswer(string boardId, string targetUserId, string answer)
    {
        var roomUsers = _roomUsers.GetValueOrDefault(boardId, new HashSet<string>());
        if (roomUsers.Contains(Context.ConnectionId) && roomUsers.Contains(targetUserId))
        {
            await Clients.Client(targetUserId).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
        }
    }

    public async Task SendIceCandidate(string boardId, string targetUserId, string candidate)
    {
        var roomUsers = _roomUsers.GetValueOrDefault(boardId, new HashSet<string>());
        if (roomUsers.Contains(Context.ConnectionId) && roomUsers.Contains(targetUserId))
        {
            await Clients.Client(targetUserId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        }
    }

    public async Task UpdateMediaStatus(string boardId, bool cameraEnabled, bool micEnabled)
    {
        var userId = Context.ConnectionId;

        if (_userMediaStatus.TryGetValue(userId, out var status))
        {
            status.CameraEnabled = cameraEnabled;
            status.MicEnabled = micEnabled;
        }

        await Clients.OthersInGroup(boardId).SendAsync("UserMediaStatusChanged",
            userId, cameraEnabled, micEnabled);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.ConnectionId;

        var roomsToCleanup = _roomUsers.Where(kvp => kvp.Value.Contains(userId)).Select(kvp => kvp.Key).ToList();

        foreach (var boardId in roomsToCleanup)
        {
            await LeaveConference(boardId);
        }

        _userMediaStatus.TryRemove(userId, out _);

        await base.OnDisconnectedAsync(exception);
    }
}
