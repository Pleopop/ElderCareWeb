namespace ElderCare.Application.Features.Chat.DTOs;

public class ConversationDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string Type { get; set; } = string.Empty;
    public Guid? BookingId { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public string? LastMessageContent { get; set; }
    public int UnreadCount { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class ParticipantDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsEdited { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? AttachmentType { get; set; }
}

public class CreateConversationRequest
{
    public List<Guid> ParticipantIds { get; set; } = new();
    public Guid? BookingId { get; set; }
    public string? Title { get; set; }
}

public class SendMessageRequest
{
    public string Content { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
    public string? AttachmentType { get; set; }
}

public class EditMessageRequest
{
    public string Content { get; set; } = string.Empty;
}
