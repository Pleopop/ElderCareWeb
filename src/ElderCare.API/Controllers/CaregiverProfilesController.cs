using ElderCare.Application.Features.Profiles.Queries;
using ElderCare.Application.Features.Profiles.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ElderCare.Domain.Interfaces;
using ElderCare.Application.Common.Interfaces;

namespace ElderCare.API.Controllers;

[Authorize(Roles = "Caregiver")]
[ApiController]
[Route("api/v1/[controller]")]
public class CaregiverProfilesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CaregiverProfilesController(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Get current caregiver's profile
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Unauthorized();

        var caregiver = await _unitOfWork.Caregivers.FirstOrDefaultAsync(
            c => c.UserId == userId.Value);

        if (caregiver == null)
            return NotFound(new { isSuccess = false, message = "Caregiver profile not found" });

        var dto = new CaregiverProfileDto
        {
            Id = caregiver.Id,
            UserId = caregiver.UserId,
            FullName = caregiver.FullName,
            Bio = caregiver.Bio,
            ExperienceYears = caregiver.ExperienceYears,
            HourlyRate = caregiver.HourlyRate,
            PersonalityType = caregiver.PersonalityType,
            AverageRating = caregiver.AverageRating ?? 0.0,
            TotalReviews = caregiver.TotalReviews,
            Address = caregiver.Address,
            Latitude = caregiver.Latitude,
            Longitude = caregiver.Longitude,
            ServiceRadiusKm = caregiver.ServiceRadiusKm,
            VerificationStatus = caregiver.VerificationStatus,
            RejectionReason = caregiver.RejectionReason,
            ApprovedAt = caregiver.ApprovedAt,
            IdentityNumber = caregiver.IdentityNumber,
            IdentityImageUrl = caregiver.IdentityImageUrl,
            IdentityBackImageUrl = caregiver.IdentityBackImageUrl,
            SelfieUrl = caregiver.SelfieUrl,
            CriminalRecordUrl = caregiver.CriminalRecordUrl,
        };

        return Ok(new { isSuccess = true, data = dto, message = "Profile retrieved successfully" });
    }

    /// <summary>
    /// Get caregiver's skills
    /// </summary>
    [HttpGet("me/skills")]
    public async Task<IActionResult> GetMySkills()
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Unauthorized();

        var caregiver = await _unitOfWork.Caregivers.FirstOrDefaultAsync(
            c => c.UserId == userId.Value);

        if (caregiver == null)
            return NotFound(new { isSuccess = false, message = "Caregiver profile not found" });

        var skills = caregiver.Skills.Select(s => new CaregiverSkillDto
        {
            Id = s.Id,
            CaregiverId = s.CaregiverId,
            SkillName = s.SkillName,
            Description = s.Description,
            ProficiencyLevel = s.ProficiencyLevel,
            CertificateUrl = s.CertificateUrl,
        }).ToList();

        return Ok(new { isSuccess = true, data = skills, message = "Skills retrieved successfully" });
    }

    /// <summary>
    /// Get caregiver's availability schedule
    /// </summary>
    [HttpGet("me/availability")]
    public async Task<IActionResult> GetMyAvailability()
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Unauthorized();

        var caregiver = await _unitOfWork.Caregivers.FirstOrDefaultAsync(
            c => c.UserId == userId.Value);

        if (caregiver == null)
            return NotFound(new { isSuccess = false, message = "Caregiver profile not found" });

        var availability = caregiver.Availabilities.Select(a => new CaregiverAvailabilityDto
        {
            Id = a.Id,
            CaregiverId = a.CaregiverId,
            DayOfWeek = a.DayOfWeek,
            StartTime = a.StartTime.ToString(@"hh\:mm"),
            EndTime = a.EndTime.ToString(@"hh\:mm"),
            IsAvailable = a.IsAvailable,
        }).OrderBy(a => a.DayOfWeek).ToList();

        return Ok(new { isSuccess = true, data = availability, message = "Availability retrieved successfully" });
    }

    /// <summary>
    /// Get caregiver's personality assessment
    /// </summary>
    [HttpGet("me/personality")]
    public async Task<IActionResult> GetMyPersonality()
    {
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Unauthorized();

        var caregiver = await _unitOfWork.Caregivers.FirstOrDefaultAsync(
            c => c.UserId == userId.Value);

        if (caregiver == null)
            return NotFound(new { isSuccess = false, message = "Caregiver profile not found" });

        if (caregiver.PersonalityAssessment == null)
            return NotFound(new { isSuccess = false, message = "Personality assessment not found" });

        var dto = new PersonalityAssessmentDto
        {
            Id = caregiver.PersonalityAssessment.Id,
            CaregiverId = caregiver.PersonalityAssessment.CaregiverId,
            PersonalityType = caregiver.PersonalityAssessment.PersonalityType,
            ExtroversionScore = caregiver.PersonalityAssessment.ExtroversionScore,
            PatienceScore = caregiver.PersonalityAssessment.PatienceScore,
            EmpathyScore = caregiver.PersonalityAssessment.EmpathyScore,
            CommunicationScore = caregiver.PersonalityAssessment.CommunicationScore,
            FlexibilityScore = caregiver.PersonalityAssessment.FlexibilityScore,
            CompletedAt = caregiver.PersonalityAssessment.CompletedAt,
        };

        return Ok(new { isSuccess = true, data = dto, message = "Personality assessment retrieved successfully" });
    }

    /// <summary>
    /// Get caregiver by ID (public endpoint for view other caregiver profiles)
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCaregiverProfile(Guid id)
    {
        var caregiver = await _unitOfWork.Caregivers.GetByIdAsync(id);

        if (caregiver == null)
            return NotFound(new { isSuccess = false, message = "Caregiver not found" });

        var dto = new CaregiverProfileDto
        {
            Id = caregiver.Id,
            UserId = caregiver.UserId,
            FullName = caregiver.FullName,
            Bio = caregiver.Bio,
            ExperienceYears = caregiver.ExperienceYears,
            HourlyRate = caregiver.HourlyRate,
            PersonalityType = caregiver.PersonalityType,
            AverageRating = caregiver.AverageRating ?? 0.0,
            TotalReviews = caregiver.TotalReviews,
            Address = caregiver.Address,
            Latitude = caregiver.Latitude,
            Longitude = caregiver.Longitude,
            ServiceRadiusKm = caregiver.ServiceRadiusKm,
            VerificationStatus = caregiver.VerificationStatus,
        };

        return Ok(new { isSuccess = true, data = dto, message = "Profile retrieved successfully" });
    }
}

// DTOs for caregiver profiles
public class CaregiverProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int? ExperienceYears { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? PersonalityType { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int? ServiceRadiusKm { get; set; }
    public int VerificationStatus { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? IdentityNumber { get; set; }
    public string? IdentityImageUrl { get; set; }
    public string? IdentityBackImageUrl { get; set; }
    public string? SelfieUrl { get; set; }
    public string? CriminalRecordUrl { get; set; }
}

public class CaregiverSkillDto
{
    public Guid Id { get; set; }
    public Guid CaregiverId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProficiencyLevel { get; set; }
    public string? CertificateUrl { get; set; }
}

public class CaregiverAvailabilityDto
{
    public Guid Id { get; set; }
    public Guid CaregiverId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}

public class PersonalityAssessmentDto
{
    public Guid Id { get; set; }
    public Guid CaregiverId { get; set; }
    public string? PersonalityType { get; set; }
    public int? ExtroversionScore { get; set; }
    public int? PatienceScore { get; set; }
    public int? EmpathyScore { get; set; }
    public int? CommunicationScore { get; set; }
    public int? FlexibilityScore { get; set; }
    public DateTime CompletedAt { get; set; }
}
