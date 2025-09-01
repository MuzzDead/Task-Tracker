using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using TaskTracker.Application.DTOs.OpenAi;
using TaskTracker.Application.OpenAi;
using TaskTracker.Domain.Options;

namespace TaskTracker.Infrastructure.Services;

public class AzureOpenAIChatService : IChatService
{
    private readonly ChatClient _chatClient;

    private const string SystemPrompt = """
        You are an assistant for the website "TaskTracker". Answer only questions about website functionality, navigation, and usage.
        If unrelated, reply: "I can only answer questions about using the website. Please ask about the site's functionality."
        Be clear and concise.

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

    public AzureOpenAIChatService(IOptions<AzureOpenAIOptions> options)
    {
        var settings = options.Value;
        var client = new AzureOpenAIClient(
            new Uri(settings.Endpoint),
            new AzureKeyCredential(settings.ApiKey));
        _chatClient = client.GetChatClient(settings.DeploymentName);
    }

    public async Task<ChatResponse> ChatAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(SystemPrompt),
            new UserChatMessage(request.Message)
        };

        var response = await _chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);
        var aiMessage = response.Value.Content[0].Text;

        return new ChatResponse
        {
            Message = aiMessage,
        };
    }
}
