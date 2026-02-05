using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Matching.DTOs;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Interfaces;
using MediatR;

namespace ElderCare.Application.Features.Matching.Queries;

// Get top matches for a beneficiary
public record GetTopMatchesQuery(Guid BeneficiaryId, int TopN = 10) : IRequest<Result<List<MatchingResultDto>>>;

public class GetTopMatchesQueryHandler : IRequestHandler<GetTopMatchesQuery, Result<List<MatchingResultDto>>>
{
    private readonly IRepository<MatchingResult> _matchingRepo;
    private readonly IRepository<CaregiverProfile> _caregiverRepo;

    public GetTopMatchesQueryHandler(
        IRepository<MatchingResult> matchingRepo,
        IRepository<CaregiverProfile> caregiverRepo)
    {
        _matchingRepo = matchingRepo;
        _caregiverRepo = caregiverRepo;
    }

    public async Task<Result<List<MatchingResultDto>>> Handle(GetTopMatchesQuery request, CancellationToken cancellationToken)
    {
        // Get matching results for beneficiary
        var matchingResults = await _matchingRepo.GetAllAsync(
            m => m.BeneficiaryId == request.BeneficiaryId);

        if (!matchingResults.Any())
            return Result<List<MatchingResultDto>>.Failure("No matching results found. Please calculate matches first.");

        // Get top N matches
        var topMatches = matchingResults
            .OrderByDescending(m => m.OverallScore)
            .Take(request.TopN)
            .ToList();

        // Get caregiver details
        var caregiverIds = topMatches.Select(m => m.CaregiverProfileId).ToList();
        var caregivers = await _caregiverRepo.GetAllAsync(c => caregiverIds.Contains(c.Id));

        // Map to DTOs
        var dtos = topMatches.Select(m =>
        {
            var caregiver = caregivers.FirstOrDefault(c => c.Id == m.CaregiverProfileId);
            if (caregiver == null) return null;

            return new MatchingResultDto
            {
                CaregiverProfileId = m.CaregiverProfileId,
                CaregiverName = caregiver.FullName,
                OverallScore = m.OverallScore,
                PersonalityScore = m.PersonalityScore,
                SkillScore = m.SkillScore,
                AvailabilityScore = m.AvailabilityScore,
                LocationScore = m.LocationScore,
                PerformanceScore = m.PerformanceScore,
                AverageRating = caregiver.AverageRating ?? 0.0,
                TotalReviews = caregiver.TotalReviews,
                HourlyRate = caregiver.HourlyRate,
                YearsOfExperience = caregiver.ExperienceYears
            };
        })
        .Where(dto => dto != null)
        .ToList()!;

        return Result<List<MatchingResultDto>>.Success(dtos, $"Found top {dtos.Count} matches");
    }
}
