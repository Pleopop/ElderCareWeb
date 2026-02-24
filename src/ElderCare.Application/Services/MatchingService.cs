using ElderCare.Application.Common.Interfaces;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;

namespace ElderCare.Application.Services;

public class MatchingService : IMatchingService
{
    private readonly IRepository<PersonalityAssessment> _personalityRepo;
    private readonly IRepository<CaregiverSkill> _skillRepo;
    private readonly IRepository<CaregiverAvailability> _availabilityRepo;
    private readonly IRepository<BeneficiaryPreference> _preferenceRepo;
    private readonly IRepository<Review> _reviewRepo;

    public MatchingService(
        IRepository<PersonalityAssessment> personalityRepo,
        IRepository<CaregiverSkill> skillRepo,
        IRepository<CaregiverAvailability> availabilityRepo,
        IRepository<BeneficiaryPreference> preferenceRepo,
        IRepository<Review> reviewRepo)
    {
        _personalityRepo = personalityRepo;
        _skillRepo = skillRepo;
        _availabilityRepo = availabilityRepo;
        _preferenceRepo = preferenceRepo;
        _reviewRepo = reviewRepo;
    }

    public async Task<MatchingResult> CalculateMatchAsync(
        Guid beneficiaryId,
        Guid CaregiverId,
        Caregiver caregiver,
        Beneficiary beneficiary)
    {
        // Calculate individual scores
        var personalityScore = await CalculatePersonalityScoreAsync(CaregiverId, beneficiaryId);
        var skillScore = await CalculateSkillScoreAsync(CaregiverId, beneficiaryId);
        var availabilityScore = CalculateAvailabilityScore(caregiver);
        var locationScore = CalculateLocationScore(caregiver, beneficiary);
        var performanceScore = await CalculatePerformanceScoreAsync(CaregiverId);

        // Weighted overall score
        var overallScore = (
            personalityScore * 0.30 +
            skillScore * 0.25 +
            availabilityScore * 0.15 +
            locationScore * 0.10 +
            performanceScore * 0.20
        );

        return new MatchingResult
        {
            BeneficiaryId = beneficiaryId,
            CaregiverId = CaregiverId,
            OverallScore = overallScore,
            PersonalityScore = personalityScore,
            SkillScore = skillScore,
            AvailabilityScore = availabilityScore,
            LocationScore = locationScore,
            PerformanceScore = performanceScore,
            CalculatedAt = DateTime.UtcNow
        };
    }

    private async Task<double> CalculatePersonalityScoreAsync(Guid CaregiverId, Guid beneficiaryId)
    {
        // Get caregiver personality assessment
        var caregiverPersonality = await _personalityRepo
            .FirstOrDefaultAsync(p => p.CaregiverId == CaregiverId);

        if (caregiverPersonality == null)
            return 50.0; // Default score if no assessment

        // Get beneficiary preferences
        var preferences = await _preferenceRepo
            .FirstOrDefaultAsync(p => p.BeneficiaryId == beneficiaryId);

        if (preferences == null)
            return 70.0; // Default good score if no preferences

        // Personality matching based on assessment scores
        // Higher scores in key caregiving traits are better
        var score = 0f;
        
        if (caregiverPersonality.EmpathyScore.HasValue)
            score += caregiverPersonality.EmpathyScore.Value * 0.35f;
        else
            score += 70 * 0.35f; // Default
            
        if (caregiverPersonality.PatienceScore.HasValue)
            score += caregiverPersonality.PatienceScore.Value * 0.30f;
        else
            score += 70 * 0.30f;
            
        if (caregiverPersonality.CommunicationScore.HasValue)
            score += caregiverPersonality.CommunicationScore.Value * 0.20f;
        else
            score += 70 * 0.20f;
            
        if (caregiverPersonality.FlexibilityScore.HasValue)
            score += caregiverPersonality.FlexibilityScore.Value * 0.10f;
        else
            score += 70 * 0.10f;
            
        if (caregiverPersonality.ExtroversionScore.HasValue)
            score += caregiverPersonality.ExtroversionScore.Value * 0.05f;
        else
            score += 70 * 0.05f;

        return Math.Clamp(score, 0, 100);
    }

    private async Task<double> CalculateSkillScoreAsync(Guid CaregiverId, Guid beneficiaryId)
    {
        // Get caregiver skills
        var caregiverSkills = await _skillRepo
            .GetAllAsync(s => s.CaregiverId == CaregiverId);

        if (!caregiverSkills.Any())
            return 40.0; // Low score if no skills listed

        // Get beneficiary preferences for required skills
        var preferences = await _preferenceRepo
            .FirstOrDefaultAsync(p => p.BeneficiaryId == beneficiaryId);

        if (preferences == null || string.IsNullOrEmpty(preferences.SpecialRequirements))
            return 70.0; // Default if no specific requirements

        // Parse special requirements (comma-separated skills/requirements)
        var requiredSkills = preferences.SpecialRequirements
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim().ToLower())
            .ToList();

        if (!requiredSkills.Any())
            return 70.0;

        // Calculate match percentage
        var caregiverSkillNames = caregiverSkills
            .Select(s => s.SkillName.ToLower())
            .ToList();

        var matchedSkills = requiredSkills
            .Count(rs => caregiverSkillNames.Any(cs => cs.Contains(rs) || rs.Contains(cs)));

        var matchPercentage = (double)matchedSkills / requiredSkills.Count * 100;

        return Math.Clamp(matchPercentage, 0, 100);
    }

    private double CalculateAvailabilityScore(Caregiver caregiver)
    {
        // Simple availability score based on verification status and experience
        if (caregiver.VerificationStatus != Domain.Enums.VerificationStatus.Approved)
            return 0.0;

        var score = 50.0; // Base score for approved caregiver

        // Bonus for experience
        if (caregiver.ExperienceYears.HasValue)
        {
            score += Math.Min(caregiver.ExperienceYears.Value * 5, 30); // Max 30 points for experience
        }

        // Bonus for hourly rate (lower rate = higher score)
        if (caregiver.HourlyRate.HasValue)
        {
            if (caregiver.HourlyRate.Value < 50000)
                score += 20;
            else if (caregiver.HourlyRate.Value < 100000)
                score += 10;
        }

        return Math.Clamp(score, 0, 100);
    }

    private double CalculateLocationScore(Caregiver caregiver, Beneficiary beneficiary)
    {
        // TODO: Implement actual distance calculation when location data is available
        // For now, return a default score
        return 75.0;
    }

    private async Task<double> CalculatePerformanceScoreAsync(Guid CaregiverId)
    {
        // Get caregiver's reviews
        var reviews = await _reviewRepo
            .GetAllAsync(r => r.CaregiverId == CaregiverId);

        if (!reviews.Any())
            return 60.0; // Default score for new caregivers

        // Calculate average rating
        var avgRating = reviews.Average(r => r.OverallRating);

        // Convert 1-5 rating to 0-100 score
        var score = (avgRating / 5.0) * 100;

        return Math.Clamp(score, 0, 100);
    }
}
