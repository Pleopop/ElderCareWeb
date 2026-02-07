using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Features.Chat.DTOs;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElderCare.Application.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;

    public ChatService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Conversations

    public async Task<ConversationDto> CreateConversationAsync(Guid userId, List<Guid> participantIds, Guid? bookingId = null, string? title = null)
    {
        // Ensure creator is in participants
        if (!participantIds.Contains(userId))
            participantIds.Add(userId);

        // Check if 1-on-1 conversation already exists
        if (participantIds.Count == 2)
        {
            var existingConversation = await FindExistingOneOnOneConversationAsync(participantIds[0], participantIds[1]);
            if (existingConversation != null)
                return await MapToConversationDto(existingConversation, userId);
        }

        var conversation = new Conversation
        {
            Title = title,
            Type = participantIds.Count == 2 ? ConversationType.OneOnOne : ConversationType.Group,
            BookingId = bookingId,
            IsArchived = false
        };

        await _unitOfWork.Repository<Conversation>().AddAsync(conversation);
        await _unitOfWork.SaveChangesAsync();

        // Add participants
        foreach (var participantId in participantIds)
        {
            var participant = new ConversationParticipant
            {
                ConversationId = conversation.Id,
                UserId = participantId,
                JoinedAt = DateTime.UtcNow,
                UnreadCount = 0,
                IsMuted = false
            };
            await _unitOfWork.Repository<ConversationParticipant>().AddAsync(participant);
        }

        await _unitOfWork.SaveChangesAsync();

        return await MapToConversationDto(conversation, userId);
    }

    public async Task<List<ConversationDto>> GetUserConversationsAsync(Guid userId)
    {
        var participants = await _unitOfWork.Repository<ConversationParticipant>()
            .GetAllAsync(cp => cp.UserId == userId);

        var conversationIds = participants.Select(p => p.ConversationId).ToList();

        var conversations = await _unitOfWork.Repository<Conversation>()
            .GetAllAsync(c => conversationIds.Contains(c.Id) && !c.IsArchived);

        var result = new List<ConversationDto>();
        foreach (var conversation in conversations.OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt))
        {
            result.Add(await MapToConversationDto(conversation, userId));
        }

        return result;
    }

    public async Task<ConversationDto> GetConversationAsync(Guid conversationId, Guid userId)
    {
        var conversation = await _unitOfWork.Repository<Conversation>().GetByIdAsync(conversationId);
        if (conversation == null)
            throw new Exception("Conversation not found");

        // Verify user is participant
        var isParticipant = await _unitOfWork.Repository<ConversationParticipant>()
            .GetAllAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);
        
        if (!isParticipant.Any())
            throw new UnauthorizedAccessException("User is not a participant in this conversation");

        return await MapToConversationDto(conversation, userId);
    }

    public async Task ArchiveConversationAsync(Guid conversationId, Guid userId)
    {
        var conversation = await _unitOfWork.Repository<Conversation>().GetByIdAsync(conversationId);
        if (conversation == null)
            throw new Exception("Conversation not found");

        conversation.IsArchived = true;
        await _unitOfWork.Repository<Conversation>().UpdateAsync(conversation);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion

    #region Messages

    public async Task<MessageDto> SendMessageAsync(Guid conversationId, Guid senderId, string content, string? attachmentUrl = null, string? attachmentType = null)
    {
        // Verify sender is participant
        var isParticipant = await _unitOfWork.Repository<ConversationParticipant>()
            .GetAllAsync(cp => cp.ConversationId == conversationId && cp.UserId == senderId);
        
        if (!isParticipant.Any())
            throw new UnauthorizedAccessException("User is not a participant in this conversation");

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            Status = MessageStatus.Sent,
            SentAt = DateTime.UtcNow,
            AttachmentUrl = attachmentUrl,
            AttachmentType = attachmentType,
            IsEdited = false
        };

        await _unitOfWork.Repository<Message>().AddAsync(message);

        // Update conversation last message
        var conversation = await _unitOfWork.Repository<Conversation>().GetByIdAsync(conversationId);
        if (conversation != null)
        {
            conversation.LastMessageId = message.Id;
            conversation.LastMessageAt = message.SentAt;
            await _unitOfWork.Repository<Conversation>().UpdateAsync(conversation);
        }

        // Increment unread count for other participants
        var participants = await _unitOfWork.Repository<ConversationParticipant>()
            .GetAllAsync(cp => cp.ConversationId == conversationId && cp.UserId != senderId);
        
        foreach (var participant in participants)
        {
            participant.UnreadCount++;
            await _unitOfWork.Repository<ConversationParticipant>().UpdateAsync(participant);
        }

        await _unitOfWork.SaveChangesAsync();

        return await MapToMessageDto(message);
    }

    public async Task<List<MessageDto>> GetMessagesAsync(Guid conversationId, Guid userId, int page = 1, int pageSize = 50)
    {
        // Verify user is participant
        var isParticipant = await _unitOfWork.Repository<ConversationParticipant>()
            .GetAllAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);
        
        if (!isParticipant.Any())
            throw new UnauthorizedAccessException("User is not a participant in this conversation");

        var allMessages = await _unitOfWork.Repository<Message>()
            .GetAllAsync(m => m.ConversationId == conversationId);

        var messages = allMessages
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(m => m.SentAt)
            .ToList();

        var result = new List<MessageDto>();
        foreach (var message in messages)
        {
            result.Add(await MapToMessageDto(message));
        }

        return result;
    }

    public async Task<MessageDto> EditMessageAsync(Guid messageId, Guid userId, string newContent)
    {
        var message = await _unitOfWork.Repository<Message>().GetByIdAsync(messageId);
        if (message == null)
            throw new Exception("Message not found");

        if (message.SenderId != userId)
            throw new UnauthorizedAccessException("Only the sender can edit this message");

        message.Content = newContent;
        message.IsEdited = true;
        message.EditedAt = DateTime.UtcNow;

        await _unitOfWork.Repository<Message>().UpdateAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return await MapToMessageDto(message);
    }

    public async Task DeleteMessageAsync(Guid messageId, Guid userId)
    {
        var message = await _unitOfWork.Repository<Message>().GetByIdAsync(messageId);
        if (message == null)
            throw new Exception("Message not found");

        if (message.SenderId != userId)
            throw new UnauthorizedAccessException("Only the sender can delete this message");

        await _unitOfWork.Repository<Message>().DeleteAsync(message);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion

    #region Read Receipts

    public async Task MarkAsReadAsync(Guid conversationId, Guid userId)
    {
        var participant = (await _unitOfWork.Repository<ConversationParticipant>()
            .GetAllAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId))
            .FirstOrDefault();

        if (participant == null)
            throw new Exception("Participant not found");

        participant.LastReadAt = DateTime.UtcNow;
        participant.UnreadCount = 0;

        await _unitOfWork.Repository<ConversationParticipant>().UpdateAsync(participant);

        // Update message status to Read
        var unreadMessages = await _unitOfWork.Repository<Message>()
            .GetAllAsync(m => m.ConversationId == conversationId && 
                             m.SenderId != userId && 
                             m.Status != MessageStatus.Read);

        foreach (var message in unreadMessages)
        {
            message.Status = MessageStatus.Read;
            message.ReadAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Message>().UpdateAsync(message);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        var participants = await _unitOfWork.Repository<ConversationParticipant>()
            .GetAllAsync(cp => cp.UserId == userId);

        return participants.Sum(p => p.UnreadCount);
    }

    #endregion

    #region Helper Methods

    private async Task<Conversation?> FindExistingOneOnOneConversationAsync(Guid user1Id, Guid user2Id)
    {
        var user1Conversations = await _unitOfWork.Repository<ConversationParticipant>()
            .GetAllAsync(cp => cp.UserId == user1Id);

        var user2Conversations = await _unitOfWork.Repository<ConversationParticipant>()
            .GetAllAsync(cp => cp.UserId == user2Id);

        var commonConversationIds = user1Conversations
            .Select(cp => cp.ConversationId)
            .Intersect(user2Conversations.Select(cp => cp.ConversationId))
            .ToList();

        foreach (var conversationId in commonConversationIds)
        {
            var conversation = await _unitOfWork.Repository<Conversation>().GetByIdAsync(conversationId);
            if (conversation != null && conversation.Type == ConversationType.OneOnOne && !conversation.IsArchived)
            {
                var participantCount = (await _unitOfWork.Repository<ConversationParticipant>()
                    .GetAllAsync(cp => cp.ConversationId == conversationId)).Count();
                
                if (participantCount == 2)
                    return conversation;
            }
        }

        return null;
    }

    private async Task<ConversationDto> MapToConversationDto(Conversation conversation, Guid currentUserId)
    {
        var participants = await _unitOfWork.Repository<ConversationParticipant>()
            .GetAllAsync(cp => cp.ConversationId == conversation.Id);

        var currentUserParticipant = participants.FirstOrDefault(p => p.UserId == currentUserId);

        var participantDtos = new List<ParticipantDto>();
        foreach (var participant in participants)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(participant.UserId);
            if (user != null)
            {
                participantDtos.Add(new ParticipantDto
                {
                    UserId = user.Id,
                    FullName = user.Email,
                    Email = user.Email,
                    Role = user.Role.ToString()
                });
            }
        }

        string? lastMessageContent = null;
        if (conversation.LastMessageId.HasValue)
        {
            var lastMessage = await _unitOfWork.Repository<Message>().GetByIdAsync(conversation.LastMessageId.Value);
            lastMessageContent = lastMessage?.Content;
        }

        return new ConversationDto
        {
            Id = conversation.Id,
            Title = conversation.Title,
            Type = conversation.Type.ToString(),
            BookingId = conversation.BookingId,
            LastMessageAt = conversation.LastMessageAt,
            LastMessageContent = lastMessageContent,
            UnreadCount = currentUserParticipant?.UnreadCount ?? 0,
            Participants = participantDtos,
            CreatedAt = conversation.CreatedAt
        };
    }

    private async Task<MessageDto> MapToMessageDto(Message message)
    {
        var sender = await _unitOfWork.Users.GetByIdAsync(message.SenderId);

        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderName = sender?.Email ?? "Unknown",
            Content = message.Content,
            Status = message.Status.ToString(),
            SentAt = message.SentAt,
            DeliveredAt = message.DeliveredAt,
            ReadAt = message.ReadAt,
            IsEdited = message.IsEdited,
            AttachmentUrl = message.AttachmentUrl,
            AttachmentType = message.AttachmentType
        };
    }

    #endregion
}
