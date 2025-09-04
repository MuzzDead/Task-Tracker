using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Chat;
using TaskTracker.Application.DTOs.OpenAi;
using TaskTracker.Application.OpenAi;
using TaskTracker.Domain.Options;

namespace TaskTracker.Infrastructure.Services;

public class AzureOpenAIChatService : IChatService
{
    private readonly ChatClient _chatClient;
    private readonly IChatSessionStore _store;

    private const string SystemPrompt = """
        You are an assistant for the website "TaskTracker". Answer only questions about website functionality, navigation, and usage.
        If unrelated, reply: "I can only answer questions about using the website. Please ask about the site's functionality."
        Be clear and concise. Be friendly and welcoming.

        Instructions:
            { "president", "Vasyl Mahometa :)" },
            { "who is the president", "Vasyl Mahometa :)" },

            { "register", "Go to 'Sign Up' in the bottom left navigation bar. The login form will open. Click 'Register' to switch to the registration form." },
            { "login", "Go to 'Sign Up' in the bottom left navigation bar. The login form will open. Enter your credentials and click 'Sign In'." },

            { "create board", "Go to 'Boards' in the bottom left navigation bar. On the boards page, click 'Add Board' in the top right to open the board creation form." },
            { "add board", "Go to 'Boards' in the bottom left navigation bar. On the boards page, click 'Add Board' in the top right to open the board creation form." },

            { "add new column", "On a specific board page, click 'Add Column' in the top right. A column creation form will open." },
            { "create column", "On a specific board page, click 'Add Column' in the top right. A column creation form will open." },

            { "add card", "On a board page, within the desired column, click 'Add Card'. Enter the card name in the field that appears." },

            { "add members", "On a board page, click 'Members' in the top right. Then click 'Invite Members', enter the email address, and send the invite." },

            { "add assignee", "Open the desired card, click 'Assign user', and select a member from the list." }
        """;

    public AzureOpenAIChatService(IOptions<AzureOpenAIOptions> options, IChatSessionStore store)
    {
        _store = store;

        var settings = options.Value;
        var client = new AzureOpenAIClient(
            new Uri(settings.Endpoint),
            new AzureKeyCredential(settings.ApiKey));
        _chatClient = client.GetChatClient(settings.DeploymentName);
    }

    public async Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var sessionId = request.SessionId ?? Guid.NewGuid().ToString();
        var history = _store.GetOrCreate(sessionId);

        var messages = new List<ChatMessage> { new SystemChatMessage(SystemPrompt) };

        foreach (var msg in history)
        {
            if (msg.Role == AuthorRole.User)
                messages.Add(new UserChatMessage(msg.Content));
            else if (msg.Role == AuthorRole.Assistant)
                messages.Add(new AssistantChatMessage(msg.Content));
        }

        messages.Add(new UserChatMessage(request.Message));
        _store.AppendUser(sessionId, request.Message);

        var response = await _chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        var aiText = response.Value.Content[0].Text;

        _store.AppendAssistant(sessionId, aiText);

        return new ChatResponse { Message = aiText, SessionId = sessionId };
    }

    public Task<ChatHistoryApiResponse> GetHistoryAsync(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentException("SessionId is required", nameof(sessionId));

        var history = _store.GetOrCreate(sessionId);

        var messages = history
            .Where(msg => msg.Role != AuthorRole.System)
            .Select(msg => new ChatHistoryResponse
            {
                Role = msg.Role.ToString()!,
                Content = msg.Content,
                Timestamp = DateTime.UtcNow
            })
            .ToList();

        var result = new ChatHistoryApiResponse
        {
            SessionId = sessionId,
            Messages = messages
        };

        return Task.FromResult(result);
    }
}
