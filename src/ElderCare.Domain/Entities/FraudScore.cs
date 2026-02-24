namespace ElderCare.Domain.Entities;

/// <summary>
/// Represents a fraud risk score for a user
/// </summary>
public class FraudScore : BaseEntity
{
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Overall fraud risk score (0-100, higher = more suspicious)
    /// </summary>
    public decimal OverallScore { get; set; }
    
    /// <summary>
    /// GPS-related fraud score
    /// </summary>
    public decimal GPSScore { get; set; }
    
    /// <summary>
    /// Booking pattern fraud score
    /// </summary>
    public decimal BookingScore { get; set; }
    
    /// <summary>
    /// Payment fraud score
    /// </summary>
    public decimal PaymentScore { get; set; }
    
    /// <summary>
    /// Identity verification fraud score
    /// </summary>
    public decimal IdentityScore { get; set; }
    
    public DateTime CalculatedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
}
