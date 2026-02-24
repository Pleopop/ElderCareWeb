using ElderCare.Domain.Enums;

namespace ElderCare.Domain.Entities;

/// <summary>
/// Represents a dispute filed by a customer or caregiver regarding a booking
/// </summary>
public class Dispute : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid InitiatedBy { get; set; }
    public Guid RespondentId { get; set; }
    public DisputeType DisputeType { get; set; }
    public DisputeStatus Status { get; set; } = DisputeStatus.Pending;
    public int Priority { get; set; } = 2; // 1=Low, 2=Medium, 3=High, 4=Critical
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? RequestedAmount { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public DateTime FiledAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
    public string? Resolution { get; set; }
    public string? ResolutionNotes { get; set; }

    // Navigation properties
    public virtual Booking Booking { get; set; } = null!; 
    public virtual User Initiator { get; set; } = null!;
    public virtual User Respondent { get; set; } = null!;
    public virtual User? Reviewer { get; set; }
    public ICollection<DisputeEvidence> Evidence { get; set; } = new List<DisputeEvidence>();
    public ICollection<DisputeMessage> Messages { get; set; } = new List<DisputeMessage>();
}
