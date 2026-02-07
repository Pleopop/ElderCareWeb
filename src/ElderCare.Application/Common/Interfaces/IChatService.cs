using ElderCare.Application.Features.Chat.DTOs;

namespace ElderCare.Application.Common.Interfaces;

public interface IChatService
{
    // Conversations
    Task<ConversationDto> CreateConversationAsync(Guid userId, List<Guid> participantIds, Guid? bookingId = null, string? title = null);
    Task<List<ConversationDto>> GetUserConversationsAsync(Guid userId);
    Task<ConversationDto> GetConversationAsync(Guid conversationId, Guid userId);
    Task ArchiveConversationAsync(Guid conversationId, Guid userId);
    
    // Messages
    Task<MessageDto> SendMessageAsync(Guid conversationId, Guid senderId, string content, string? attachmentUrl = null, string? attachmentType = null);
    Task<List<MessageDto>> GetMessagesAsync(Guid conversationId, Guid userId, int page = 1, int pageSize = 50);
    Task<MessageDto> EditMessageAsync(Guid messageId, Guid userId, string newContent);
    Task DeleteMessageAsync(Guid messageId, Guid userId);
    
    // Read Receipts
    Task MarkAsReadAsync(Guid conversationId, Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
}
