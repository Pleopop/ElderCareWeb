namespace ElderCare.Domain.Entities;

/// <summary>
/// Caregiver's observation notes about beneficiary's daily condition
/// Used by AI to assess mood and provide recommendations
/// </summary>
public class CareNote : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid CaregiverId { get; set; }
    public Guid BeneficiaryId { get; set; }
    
    // Observation details
    public string Observation { get; set; } = string.Empty;
    public Enums.MoodLevel AssessedMood { get; set; }
    public DateTime ObservedAt { get; set; }
    
    // AI Analysis results
    public string? AiMoodAnalysis { get; set; }
    public double? SentimentScore { get; set; } // -1.0 to 1.0
    public List<string>? DetectedEmotions { get; set; } // JSON array: ["happy", "calm", "engaged"]
    public List<string>? SuggestedActions { get; set; } // JSON array of AI suggestions
    
    // Flags
    public bool RequiresAttention { get; set; } // AI detected concerning pattern
    public bool NotifiedCustomer { get; set; }
    
    // Navigation properties
    public Booking Booking { get; set; } = null!;
    public Caregiver Caregiver { get; set; } = null!;
    public Beneficiary Beneficiary { get; set; } = null!;
}
