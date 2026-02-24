using ElderCare.Domain.Enums;

namespace ElderCare.Domain.Entities;

/// <summary>
/// Represents a message in a dispute conversation
/// </summary>
public class DisputeMessage : BaseEntity
{
    public Guid DisputeId { get; set; }
    public Guid SenderId { get; set; }
    public string MessageText { get; set; } = string.Empty;
    public bool IsAdminMessage { get; set; }
    public DateTime SentAt { get; set; }
    
    // Navigation properties
    public Dispute Dispute { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
