namespace ElderCare.Domain.Entities;

/// <summary>
/// Represents a suspicious activity detected by the fraud detection system
/// </summary>
public class SuspiciousActivity : BaseEntity
{
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Type of suspicious activity
    /// </summary>
    public string ActivityType { get; set; } = string.Empty;
    
    /// <summary>
    /// JSON data with activity details
    /// </summary>
    public string Details { get; set; } = string.Empty;
    
    /// <summary>
    /// Risk score for this activity (0-100)
    /// </summary>
    public decimal RiskScore { get; set; }
    
    public DateTime DetectedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
}
