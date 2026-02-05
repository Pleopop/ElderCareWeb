using ElderCare.Domain.Enums;

namespace ElderCare.Domain.Entities;

public class Dispute : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid RaisedByUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DisputeStatus Status { get; set; } = DisputeStatus.Open;
    public string? Resolution { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    
    public Booking Booking { get; set; } = null!;
    public ICollection<DisputeEvidence> Evidences { get; set; } = new List<DisputeEvidence>();
}

public class DisputeEvidence : BaseEntity
{
    public Guid DisputeId { get; set; }
    public string EvidenceType { get; set; } = string.Empty; // Photo, Video, Document
    public string EvidenceUrl { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public Dispute Dispute { get; set; } = null!;
}
