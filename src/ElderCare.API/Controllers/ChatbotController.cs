using ElderCare.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElderCare.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ChatbotController : ControllerBase
{
    private readonly IChatbotService _chatbotService;

    public ChatbotController(IChatbotService chatbotService)
    {
        _chatbotService = chatbotService;
    }

    /// <summary>
    /// Send a message to the AI chatbot advisor
    /// </summary>
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] ChatbotRequest request, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result = await _chatbotService.AskAsync(userId, request, ct);
        return Ok(result);
    }
}
