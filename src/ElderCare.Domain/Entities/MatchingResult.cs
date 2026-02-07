namespace ElderCare.Domain.Entities;

public class MatchingResult : BaseEntity
{
    public Guid BeneficiaryId { get; set; }
    public Guid CaregiverId { get; set; }
    
    // Overall compatibility score (0-100)
    public double OverallScore { get; set; }
    
    // Individual component scores
    public double PersonalityScore { get; set; }
    public double SkillScore { get; set; }
    public double AvailabilityScore { get; set; }
    public double LocationScore { get; set; }
    public double PerformanceScore { get; set; }
    
    // Metadata
    public DateTime CalculatedAt { get; set; }
    
    // Navigation properties
    public Beneficiary Beneficiary { get; set; } = null!;
    public Caregiver Caregiver { get; set; } = null!;
}
