namespace ElderCare.Domain.Entities;

/// <summary>
/// Represents a user's participation in a conversation
/// Tracks read status and unread count per user
/// </summary>
public class ConversationParticipant : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    
    public DateTime JoinedAt { get; set; }
    public DateTime? LastReadAt { get; set; }
    public int UnreadCount { get; set; }
    public bool IsMuted { get; set; }
    
    // Navigation properties
    public Conversation Conversation { get; set; } = null!;
    public User User { get; set; } = null!;
}
