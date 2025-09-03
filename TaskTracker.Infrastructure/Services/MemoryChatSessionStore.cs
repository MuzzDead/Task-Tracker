using Microsoft.Extensions.Caching.Memory;
using Microsoft.SemanticKernel.ChatCompletion;
using TaskTracker.Application.OpenAi;

namespace TaskTracker.Infrastructure.Services;

public sealed class MemoryChatSessionStore : IChatSessionStore
{
    private readonly IMemoryCache _cache;
    private static readonly MemoryCacheEntryOptions _opts = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
        Size = 1
    };

    public MemoryChatSessionStore(IMemoryCache cache) => _cache = cache;

    public ChatHistory GetOrCreate(string sessionId)
        => _cache.GetOrCreate(sessionId, entry =>
        {
            entry.SetOptions(_opts);
            return new ChatHistory();
        })!;

    public void AppendUser(string sessionId, string message)
        => Append(sessionId, AuthorRole.User, message);

    public void AppendAssistant(string sessionId, string message)
        => Append(sessionId, AuthorRole.Assistant, message);

    private void Append(string sessionId, AuthorRole role, string content)
    {
        var history = GetOrCreate(sessionId);
        history.AddMessage(role, content);
        _cache.Set(sessionId, history, _opts);
    }
}
