using Microsoft.SemanticKernel.ChatCompletion;

namespace TaskTracker.Application.OpenAi;

public interface IChatSessionStore
{
    ChatHistory GetOrCreate(string sessionId);
    void AppendUser(string sessionId, string message);
    void AppendAssistant(string sessionId, string message);
}
