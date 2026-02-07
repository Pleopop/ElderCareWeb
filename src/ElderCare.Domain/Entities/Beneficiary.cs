using ElderCare.Domain.Enums;

namespace ElderCare.Domain.Entities;

public class Beneficiary : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public string? MedicalConditions { get; set; }
    public string? Allergies { get; set; }
    public CognitiveStatus? CognitiveStatus { get; set; }
    public string? PersonalityTraits { get; set; }
    public string? Hobbies { get; set; }
    public string? DailyRoutine { get; set; }
    
    public Customer Customer { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<BeneficiaryPreference> Preferences { get; set; } = new List<BeneficiaryPreference>();
}

public class BeneficiaryPreference : BaseEntity
{
    public Guid BeneficiaryId { get; set; }
    public string? PreferredGender { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? RequiredSkills { get; set; } // Comma-separated skills
    public string? PreferredPersonalityTraits { get; set; } // Comma-separated traits
    public string? PreferenceKey { get; set; }
    public string? PreferenceValue { get; set; }
    
    public Beneficiary Beneficiary { get; set; } = null!;
}
