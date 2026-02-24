namespace ElderCare.Domain.Entities;

/// <summary>
/// Represents a fraud alert for suspicious activities
/// </summary>
public class FraudAlert : BaseEntity
{
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Type of fraud alert: GPS, Booking, Payment, Identity
    /// </summary>
    public string AlertType { get; set; } = string.Empty;
    
    /// <summary>
    /// Severity level: 1=Low, 2=Medium, 3=High, 4=Critical
    /// </summary>
    public int Severity { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public DateTime DetectedAt { get; set; }
    
    /// <summary>
    /// Status: Pending, Investigating, Resolved, FalsePositive
    /// </summary>
    public string Status { get; set; } = "Pending";
    
    public string? InvestigatedBy { get; set; }
    
    public DateTime? InvestigatedAt { get; set; }
    
    public string? Resolution { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
}
