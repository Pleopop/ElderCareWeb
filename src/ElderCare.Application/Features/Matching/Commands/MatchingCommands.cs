using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Matching.DTOs;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;
using MediatR;

namespace ElderCare.Application.Features.Matching.Commands;

// Calculate matches for a beneficiary
public record CalculateMatchCommand(Guid BeneficiaryId) : IRequest<Result<List<MatchingResultDto>>>;

public class CalculateMatchCommandHandler : IRequestHandler<CalculateMatchCommand, Result<List<MatchingResultDto>>>
{
    private readonly IRepository<Beneficiary> _beneficiaryRepo;
    private readonly IRepository<Caregiver> _caregiverRepo;
    private readonly IRepository<MatchingResult> _matchingRepo;
    private readonly IMatchingService _matchingService;
    private readonly IUnitOfWork _unitOfWork;

    public CalculateMatchCommandHandler(
        IRepository<Beneficiary> beneficiaryRepo,
        IRepository<Caregiver> caregiverRepo,
        IRepository<MatchingResult> matchingRepo,
        IMatchingService matchingService,
        IUnitOfWork unitOfWork)
    {
        _beneficiaryRepo = beneficiaryRepo;
        _caregiverRepo = caregiverRepo;
        _matchingRepo = matchingRepo;
        _matchingService = matchingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<MatchingResultDto>>> Handle(CalculateMatchCommand request, CancellationToken cancellationToken)
    {
        // Get beneficiary
        var beneficiary = await _beneficiaryRepo.GetByIdAsync(request.BeneficiaryId);
        if (beneficiary == null)
            return Result<List<MatchingResultDto>>.Failure("Beneficiary not found");

        // Get all approved caregivers
        var caregivers = await _caregiverRepo.GetAllAsync(
            c => c.VerificationStatus == VerificationStatus.Approved);

        if (!caregivers.Any())
            return Result<List<MatchingResultDto>>.Failure("No approved caregivers available");

        // Calculate matches for all caregivers
        var matchingResults = new List<MatchingResult>();
        
        foreach (var caregiver in caregivers)
        {
            var matchResult = await _matchingService.CalculateMatchAsync(
                request.BeneficiaryId,
                caregiver.Id,
                caregiver,
                beneficiary);

            matchingResults.Add(matchResult);
        }

        // Delete old matching results for this beneficiary
        var oldResults = await _matchingRepo.GetAllAsync(m => m.BeneficiaryId == request.BeneficiaryId);
        foreach (var oldResult in oldResults)
        {
            await _matchingRepo.DeleteAsync(oldResult);
        }

        // Save new matching results
        foreach (var result in matchingResults)
        {
            await _matchingRepo.AddAsync(result);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTOs and sort by overall score
        var dtos = matchingResults
            .OrderByDescending(m => m.OverallScore)
            .Select(m =>
            {
                var caregiver = caregivers.First(c => c.Id == m.CaregiverId);
                return new MatchingResultDto
                {
                    CaregiverId = m.CaregiverId,
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
            .ToList();

        return Result<List<MatchingResultDto>>.Success(dtos, $"Calculated matches for {dtos.Count} caregivers");
    }
}
