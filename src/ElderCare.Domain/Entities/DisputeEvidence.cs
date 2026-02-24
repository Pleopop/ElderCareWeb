using ElderCare.Domain.Enums;

namespace ElderCare.Domain.Entities;

/// <summary>
/// Represents evidence uploaded for a dispute (images, documents, etc.)
/// </summary>
public class DisputeEvidence : BaseEntity
{
    public Guid DisputeId { get; set; }
    public Guid UploadedBy { get; set; }
    public EvidenceType EvidenceType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }
    
    // Navigation properties
    public Dispute Dispute { get; set; } = null!;
    public User Uploader { get; set; } = null!;
}
