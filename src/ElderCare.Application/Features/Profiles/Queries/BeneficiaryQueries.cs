using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Profiles.DTOs;
using ElderCare.Domain.Interfaces;
using MediatR;

namespace ElderCare.Application.Features.Profiles.Queries;

// Get My Beneficiaries
public record GetMyBeneficiariesQuery() : IRequest<Result<List<BeneficiaryDto>>>;

public class GetMyBeneficiariesQueryHandler : IRequestHandler<GetMyBeneficiariesQuery, Result<List<BeneficiaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetMyBeneficiariesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<BeneficiaryDto>>> Handle(GetMyBeneficiariesQuery request, CancellationToken cancellationToken)
    {
        // Get current customer
        var user = await _unitOfWork.Users.GetByIdAsync(_currentUserService.UserId!.Value, cancellationToken);
        if (user == null)
            return Result<List<BeneficiaryDto>>.Failure("User not found");

        var customer = await _unitOfWork.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id, cancellationToken);
        if (customer == null)
            return Result<List<BeneficiaryDto>>.Failure("Customer profile not found");

        // Get beneficiaries
        var beneficiaries = await _unitOfWork.Beneficiaries.GetAllAsync(
            b => b.CustomerId == customer.Id, 
            cancellationToken);

        var dtos = beneficiaries.Select(b => new BeneficiaryDto
        {
            Id = b.Id,
            CustomerId = b.CustomerId,
            FullName = b.FullName,
            DateOfBirth = b.DateOfBirth,
            Age = DateTime.UtcNow.Year - b.DateOfBirth.Year,
            Gender = b.Gender,
            Address = b.Address,
            MedicalConditions = b.MedicalConditions,
            Medications = b.Medications,
            Allergies = b.Allergies,
            MobilityLevel = b.MobilityLevel,
            CognitiveStatus = b.CognitiveStatus,
            SpecialNeeds = b.SpecialNeeds,
            PersonalityTraits = b.PersonalityTraits,
            Hobbies = b.Hobbies,
            CreatedAt = b.CreatedAt,
            UpdatedAt = b.UpdatedAt
        }).ToList();

        return Result<List<BeneficiaryDto>>.Success(dtos);
    }
}

// Get Beneficiary By Id
public record GetBeneficiaryByIdQuery(Guid Id) : IRequest<Result<BeneficiaryDto>>;

public class GetBeneficiaryByIdQueryHandler : IRequestHandler<GetBeneficiaryByIdQuery, Result<BeneficiaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetBeneficiaryByIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BeneficiaryDto>> Handle(GetBeneficiaryByIdQuery request, CancellationToken cancellationToken)
    {
        // Get current customer
        var user = await _unitOfWork.Users.GetByIdAsync(_currentUserService.UserId!.Value, cancellationToken);
        if (user == null)
            return Result<BeneficiaryDto>.Failure("User not found");

        var customer = await _unitOfWork.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id, cancellationToken);
        if (customer == null)
            return Result<BeneficiaryDto>.Failure("Customer profile not found");

        // Get beneficiary
        var beneficiary = await _unitOfWork.Beneficiaries.GetByIdAsync(request.Id, cancellationToken);
        if (beneficiary == null || beneficiary.CustomerId != customer.Id)
            return Result<BeneficiaryDto>.Failure("Beneficiary not found or does not belong to you");

        var dto = new BeneficiaryDto
        {
            Id = beneficiary.Id,
            CustomerId = beneficiary.CustomerId,
            FullName = beneficiary.FullName,
            DateOfBirth = beneficiary.DateOfBirth,
            Age = DateTime.UtcNow.Year - beneficiary.DateOfBirth.Year,
            Gender = beneficiary.Gender,
            Address = beneficiary.Address,
            MedicalConditions = beneficiary.MedicalConditions,
            Medications = beneficiary.Medications,
            Allergies = beneficiary.Allergies,
            MobilityLevel = beneficiary.MobilityLevel,
            CognitiveStatus = beneficiary.CognitiveStatus,
            SpecialNeeds = beneficiary.SpecialNeeds,
            PersonalityTraits = beneficiary.PersonalityTraits,
            Hobbies = beneficiary.Hobbies,
            CreatedAt = beneficiary.CreatedAt,
            UpdatedAt = beneficiary.UpdatedAt
        };

        return Result<BeneficiaryDto>.Success(dto);
    }
}

// Get Beneficiary Preferences
public record GetBeneficiaryPreferencesQuery(Guid BeneficiaryId) : IRequest<Result<BeneficiaryPreferenceDto>>;

public class GetBeneficiaryPreferencesQueryHandler 
    : IRequestHandler<GetBeneficiaryPreferencesQuery, Result<BeneficiaryPreferenceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetBeneficiaryPreferencesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BeneficiaryPreferenceDto>> Handle(
        GetBeneficiaryPreferencesQuery request, 
        CancellationToken cancellationToken)
    {
        // Get current customer
        var user = await _unitOfWork.Users.GetByIdAsync(_currentUserService.UserId!.Value, cancellationToken);
        if (user == null)
            return Result<BeneficiaryPreferenceDto>.Failure("User not found");

        var customer = await _unitOfWork.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id, cancellationToken);
        if (customer == null)
            return Result<BeneficiaryPreferenceDto>.Failure("Customer profile not found");

        // Verify beneficiary belongs to customer
        var beneficiary = await _unitOfWork.Beneficiaries.GetByIdAsync(request.BeneficiaryId, cancellationToken);
        if (beneficiary == null || beneficiary.CustomerId != customer.Id)
            return Result<BeneficiaryPreferenceDto>.Failure("Beneficiary not found or does not belong to you");

        // Get preferences
        var preferences = await _unitOfWork.BeneficiaryPreferences.FirstOrDefaultAsync(
            p => p.BeneficiaryId == request.BeneficiaryId, 
            cancellationToken);

        if (preferences == null)
            return Result<BeneficiaryPreferenceDto>.Failure("Preferences not found");

        var dto = new BeneficiaryPreferenceDto
        {
            Id = preferences.Id,
            BeneficiaryId = preferences.BeneficiaryId,
            PreferredGender = preferences.PreferredGender,
            PreferredAgeRange = preferences.PreferredAgeRange,
            PreferredPersonalityTraits = preferences.PreferredPersonalityTraits,
            AvoidPersonalityTraits = preferences.AvoidPersonalityTraits,
            SpecialRequirements = preferences.SpecialRequirements,
            CreatedAt = preferences.CreatedAt,
            UpdatedAt = preferences.UpdatedAt
        };

        return Result<BeneficiaryPreferenceDto>.Success(dto);
    }
}
