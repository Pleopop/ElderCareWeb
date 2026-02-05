using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Features.Notifications.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElderCare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Get current user's notifications (paginated)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetMyNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isRead = null,
        [FromQuery] string? category = null)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("User not authenticated");

        // Get all notifications for user
        var allNotifications = await _notificationService.GetUserNotificationsAsync(userId);
        
        // Apply filters
        var filtered = allNotifications.AsEnumerable();
        
        if (isRead.HasValue)
            filtered = filtered.Where(n => n.IsRead == isRead.Value);
        
        if (!string.IsNullOrEmpty(category))
            filtered = filtered.Where(n => n.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        // Order by created date descending
        var ordered = filtered.OrderByDescending(n => n.CreatedAt);

        // Pagination
        var paginated = ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        Response.Headers.Add("X-Total-Count", filtered.Count().ToString());
        Response.Headers.Add("X-Page", page.ToString());
        Response.Headers.Add("X-Page-Size", pageSize.ToString());

        return Ok(paginated);
    }

    /// <summary>
    /// Get unread notification count
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("User not authenticated");

        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(count);
    }

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    [HttpPost("{id}/read")]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPost("read-all")]
    public async Task<ActionResult> MarkAllAsRead()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("User not authenticated");

        await _notificationService.MarkAllAsReadAsync(userId);
        return NoContent();
    }

    /// <summary>
    /// Delete a notification
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteNotification(Guid id)
    {
        await _notificationService.DeleteNotificationAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Send notification (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<NotificationDto>> SendNotification([FromBody] SendNotificationRequest request)
    {
        var notification = await _notificationService.SendNotificationAsync(
            request.UserId,
            request.Title,
            request.Message,
            request.Category,
            request.Type,
            request.Priority,
            request.ActionUrl,
            request.RelatedEntityId,
            request.RelatedEntityType
        );

        var dto = new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type.ToString(),
            Category = notification.Category.ToString(),
            Priority = notification.Priority.ToString(),
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            CreatedAt = notification.CreatedAt,
            ActionUrl = notification.ActionUrl,
            RelatedEntityId = notification.RelatedEntityId,
            RelatedEntityType = notification.RelatedEntityType
        };

        return CreatedAtAction(nameof(GetMyNotifications), new { id = dto.Id }, dto);
    }
}
