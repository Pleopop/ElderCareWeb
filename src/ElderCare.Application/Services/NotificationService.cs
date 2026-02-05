using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Features.Notifications.DTOs;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;

namespace ElderCare.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Notification> SendNotificationAsync(Guid userId, string title, string message, string category, string type, string priority, string? actionUrl, Guid? relatedEntityId, string? relatedEntityType)
    {
        // Parse enums with fallback
        var categoryEnum = Enum.TryParse<NotificationCategory>(category, true, out var cat) ? cat : NotificationCategory.System;
        var typeEnum = Enum.TryParse<NotificationType>(type, true, out var typ) ? typ : NotificationType.Info;
        var priorityEnum = Enum.TryParse<NotificationPriority>(priority, true, out var pri) ? pri : NotificationPriority.Medium;

        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Category = categoryEnum,
            Type = typeEnum,
            Priority = priorityEnum,
            ActionUrl = actionUrl,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType,
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return notification;
    }

    public async Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId)
    {
        var notifications = await _unitOfWork.Notifications.GetAllAsync(n => n.UserId == userId);
        
        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type.ToString(),
            Category = n.Category.ToString(),
            Priority = n.Priority.ToString(),
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            CreatedAt = n.CreatedAt,
            ActionUrl = n.ActionUrl,
            RelatedEntityId = n.RelatedEntityId,
            RelatedEntityType = n.RelatedEntityType
        }).ToList();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        var notifications = await _unitOfWork.Notifications.GetAllAsync(n => 
            n.UserId == userId && !n.IsRead);
        return notifications.Count();
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _unitOfWork.Notifications.UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unreadNotifications = await _unitOfWork.Notifications.GetAllAsync(n => 
            n.UserId == userId && !n.IsRead);
        
        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _unitOfWork.Notifications.UpdateAsync(notification);
        }
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteNotificationAsync(Guid notificationId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification != null)
        {
            await _unitOfWork.Notifications.DeleteAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
