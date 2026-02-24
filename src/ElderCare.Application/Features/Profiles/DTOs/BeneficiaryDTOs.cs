using ElderCare.Domain.Enums;

namespace ElderCare.Application.Features.Profiles.DTOs;

// Request DTOs
public class CreateBeneficiaryRequest
{
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
}

public class UpdateBeneficiaryRequest
{
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
}

// Response DTOs
public class BeneficiaryDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Beneficiary Preferences DTOs
public class UpdateBeneficiaryPreferencesRequest
{
    public Gender? PreferredGender { get; set; }
    public string? PreferredAgeRange { get; set; }
    public string? PreferredPersonalityTraits { get; set; }
    public string? AvoidPersonalityTraits { get; set; }
    public string? SpecialRequirements { get; set; }
}

public class BeneficiaryPreferenceDto
{
    public Guid Id { get; set; }
    public Guid BeneficiaryId { get; set; }
    public Gender? PreferredGender { get; set; }
    public string? PreferredAgeRange { get; set; }
    public string? PreferredPersonalityTraits { get; set; }
    public string? AvoidPersonalityTraits { get; set; }
    public string? SpecialRequirements { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
