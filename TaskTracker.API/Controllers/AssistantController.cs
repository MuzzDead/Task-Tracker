using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs.OpenAi;
using TaskTracker.Application.OpenAi;

namespace TaskTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssistantController : ControllerBase
{
    private readonly IChatService _chatService;
    public AssistantController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> AskAsync([FromBody] ChatRequest request)
    {
        var response = await _chatService.ChatAsync(request);
        return Ok(response);
    }
}
