namespace ElderCare.Domain.Entities;

/// <summary>
/// Represents a message in a conversation
/// Supports text messages and optional file attachments
/// </summary>
public class Message : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    
    public string Content { get; set; } = string.Empty;
    public Enums.MessageStatus Status { get; set; }
    
    public DateTime SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? ReadAt { get; set; }
    
    // Edit tracking
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    
    // Attachments (optional)
    public string? AttachmentUrl { get; set; }
    public string? AttachmentType { get; set; } // "image", "document", "audio"
    
    // Navigation properties
    public Conversation Conversation { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
