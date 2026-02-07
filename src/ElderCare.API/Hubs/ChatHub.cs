using ElderCare.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace ElderCare.API.Hubs;

/// <summary>
/// SignalR Hub for real-time chat messaging
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private static readonly ConcurrentDictionary<string, string> _userConnections = new();

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections[userId] = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            
            // Notify others user is online
            await Clients.All.SendAsync("UserOnline", userId);
            Console.WriteLine($"User {userId} connected to ChatHub");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections.TryRemove(userId, out _);
            await Clients.All.SendAsync("UserOffline", userId);
            Console.WriteLine($"User {userId} disconnected from ChatHub");
        }
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Send a message to a conversation
    /// </summary>
    public async Task SendMessage(Guid conversationId, string content)
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new HubException("User not authenticated");
        }

        try
        {
            // Save to database
            var message = await _chatService.SendMessageAsync(conversationId, userId, content);
            
            // Broadcast to conversation participants
            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("ReceiveMessage", message);
        }
        catch (Exception ex)
        {
            throw new HubException($"Failed to send message: {ex.Message}");
        }
    }

    /// <summary>
    /// Join a conversation to receive real-time messages
    /// </summary>
    public async Task JoinConversation(Guid conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        Console.WriteLine($"User joined conversation {conversationId}");
    }

    /// <summary>
    /// Leave a conversation
    /// </summary>
    public async Task LeaveConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        Console.WriteLine($"User left conversation {conversationId}");
    }

    /// <summary>
    /// Notify others that user is typing
    /// </summary>
    public async Task Typing(Guid conversationId, bool isTyping)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Clients.OthersInGroup($"conversation_{conversationId}")
            .SendAsync("UserTyping", userId, isTyping);
    }

    /// <summary>
    /// Mark conversation messages as read
    /// </summary>
    public async Task MarkAsRead(Guid conversationId)
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new HubException("User not authenticated");
        }

        try
        {
            await _chatService.MarkAsReadAsync(conversationId, userId);
            
            // Notify sender messages are read
            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("MessagesRead", conversationId, userId);
        }
        catch (Exception ex)
        {
            throw new HubException($"Failed to mark as read: {ex.Message}");
        }
    }

    /// <summary>
    /// Get online status of users
    /// </summary>
    public bool IsUserOnline(string userId)
    {
        return _userConnections.ContainsKey(userId);
    }
}
