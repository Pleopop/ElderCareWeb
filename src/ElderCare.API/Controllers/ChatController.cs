using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Features.Chat.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElderCare.API.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Get all conversations for current user
    /// </summary>
    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationDto>>> GetConversations()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var conversations = await _chatService.GetUserConversationsAsync(userId);
        return Ok(conversations);
    }

    /// <summary>
    /// Create a new conversation
    /// </summary>
    [HttpPost("conversations")]
    public async Task<ActionResult<ConversationDto>> CreateConversation([FromBody] CreateConversationRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var conversation = await _chatService.CreateConversationAsync(
            userId,
            request.ParticipantIds,
            request.BookingId,
            request.Title
        );

        return CreatedAtAction(nameof(GetConversation), new { id = conversation.Id }, conversation);
    }

    /// <summary>
    /// Get a specific conversation
    /// </summary>
    [HttpGet("conversations/{id}")]
    public async Task<ActionResult<ConversationDto>> GetConversation(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        try
        {
            var conversation = await _chatService.GetConversationAsync(id, userId);
            return Ok(conversation);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Get messages for a conversation
    /// </summary>
    [HttpGet("conversations/{id}/messages")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        try
        {
            var messages = await _chatService.GetMessagesAsync(id, userId, page, pageSize);
            
            Response.Headers.Add("X-Page", page.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            
            return Ok(messages);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Send a message (REST fallback, prefer SignalR)
    /// </summary>
    [HttpPost("conversations/{id}/messages")]
    public async Task<ActionResult<MessageDto>> SendMessage(Guid id, [FromBody] SendMessageRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        try
        {
            var message = await _chatService.SendMessageAsync(
                id,
                userId,
                request.Content,
                request.AttachmentUrl,
                request.AttachmentType
            );

            return CreatedAtAction(nameof(GetMessages), new { id }, message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Edit a message
    /// </summary>
    [HttpPut("messages/{id}")]
    public async Task<ActionResult<MessageDto>> EditMessage(Guid id, [FromBody] EditMessageRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        try
        {
            var message = await _chatService.EditMessageAsync(id, userId, request.Content);
            return Ok(message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Delete a message
    /// </summary>
    [HttpDelete("messages/{id}")]
    public async Task<ActionResult> DeleteMessage(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        try
        {
            await _chatService.DeleteMessageAsync(id, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Archive a conversation
    /// </summary>
    [HttpPost("conversations/{id}/archive")]
    public async Task<ActionResult> ArchiveConversation(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        await _chatService.ArchiveConversationAsync(id, userId);
        return NoContent();
    }

    /// <summary>
    /// Get total unread message count
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var count = await _chatService.GetUnreadCountAsync(userId);
        return Ok(count);
    }

    /// <summary>
    /// Mark conversation as read
    /// </summary>
    [HttpPost("conversations/{id}/mark-read")]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        await _chatService.MarkAsReadAsync(id, userId);
        return NoContent();
    }
}
