namespace ElderCare.Domain.Entities;

/// <summary>
/// AI-generated activity suggestions for caregivers to engage beneficiaries
/// Based on beneficiary's personality, mood trends, and preferences
/// </summary>
public class ActivitySuggestion : BaseEntity
{
    public Guid BeneficiaryId { get; set; }
    
    // Suggestion details
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Enums.ActivityCategory Category { get; set; }
    public int DurationMinutes { get; set; }
    public Enums.DifficultyLevel Difficulty { get; set; }
    
    // AI reasoning
    public string? AiReasoning { get; set; } // Why this activity was suggested
    public double ConfidenceScore { get; set; } // 0.0 to 1.0
    
    // Personalization factors
    public List<string>? BasedOnTraits { get; set; } // JSON: ["outgoing", "creative"]
    public List<string>? BasedOnHobbies { get; set; } // JSON: ["gardening", "music"]
    
    // Tracking
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CaregiverFeedback { get; set; }
    public int? BeneficiaryEngagementRating { get; set; } // 1-5
    
    public DateTime GeneratedAt { get; set; }
    public DateTime? ExpiresAt { get; set; } // Suggestions expire after 7 days
    
    // Navigation properties
    public Beneficiary Beneficiary { get; set; } = null!;
}
