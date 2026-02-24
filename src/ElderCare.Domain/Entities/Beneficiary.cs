using ElderCare.Domain.Enums;

namespace ElderCare.Domain.Entities;

public class Beneficiary : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? Address { get; set; }
    public string? MedicalConditions { get; set; }
    public string? Medications { get; set; }
    public string? Allergies { get; set; }
    public MobilityLevel? MobilityLevel { get; set; }
    public CognitiveStatus? CognitiveStatus { get; set; }
    public string? SpecialNeeds { get; set; }
    public string? PersonalityTraits { get; set; }
    public string? Hobbies { get; set; }
    
    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<BeneficiaryPreference> Preferences { get; set; } = new List<BeneficiaryPreference>();
}

public class BeneficiaryPreference : BaseEntity
{
    public Guid BeneficiaryId { get; set; }
    public Gender? PreferredGender { get; set; }
    public string? PreferredAgeRange { get; set; }
    public string? PreferredPersonalityTraits { get; set; }
    public string? AvoidPersonalityTraits { get; set; }
    public string? SpecialRequirements { get; set; }
    
    // Navigation property
    public Beneficiary Beneficiary { get; set; } = null!;
}
