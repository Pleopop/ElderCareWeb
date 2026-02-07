using ElderCare.Domain.Enums;

namespace ElderCare.Domain.Entities;

public class Caregiver : BaseEntity
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? IdentityNumber { get; set; }
    public string? IdentityImageUrl { get; set; }
    public string? IdentityBackImageUrl { get; set; }
    public string? SelfieUrl { get; set; }
    public string? CriminalRecordUrl { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public string? RejectionReason { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    
    // Profile details
    public string? Bio { get; set; }
    public int? ExperienceYears { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? PersonalityType { get; set; }
    public double? AverageRating { get; set; }
    public int TotalReviews { get; set; } = 0;
    
    // Location
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? ServiceRadiusKm { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<CaregiverSkill> Skills { get; set; } = new List<CaregiverSkill>();
    public ICollection<CaregiverAvailability> Availabilities { get; set; } = new List<CaregiverAvailability>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public PersonalityAssessment? PersonalityAssessment { get; set; }
}

public class CaregiverSkill : BaseEntity
{
    public Guid CaregiverId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProficiencyLevel { get; set; } = 1; // 1-5
    public string? CertificateUrl { get; set; }
    
    public Caregiver Caregiver { get; set; } = null!;
}

public class CaregiverAvailability : BaseEntity
{
    public Guid CaregiverId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    
    public Caregiver Caregiver { get; set; } = null!;
}

public class PersonalityAssessment : BaseEntity
{
    public Guid CaregiverId { get; set; }
    public string? PersonalityType { get; set; }
    public int? ExtroversionScore { get; set; }
    public int? PatienceScore { get; set; }
    public int? EmpathyScore { get; set; }
    public int? CommunicationScore { get; set; }
    public int? FlexibilityScore { get; set; }
    public string? AssessmentJson { get; set; }
    public DateTime CompletedAt { get; set; }
    
    public Caregiver Caregiver { get; set; } = null!;
}
