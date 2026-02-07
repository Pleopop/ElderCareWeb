namespace ElderCare.Domain.Entities;

/// <summary>
/// Represents a conversation between users (Customer-Caregiver, Customer-Admin, etc.)
/// </summary>
public class Conversation : BaseEntity
{
    public string? Title { get; set; } // Optional, for group chats
    public Enums.ConversationType Type { get; set; }
    public Guid? BookingId { get; set; } // Optional link to booking context
    
    // Last message tracking
    public Guid? LastMessageId { get; set; }
    public DateTime? LastMessageAt { get; set; }
    
    public bool IsArchived { get; set; }
    
    // Navigation properties
    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public Booking? Booking { get; set; }
}
