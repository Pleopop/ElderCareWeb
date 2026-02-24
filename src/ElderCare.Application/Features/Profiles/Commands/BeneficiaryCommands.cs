using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Profiles.DTOs;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace ElderCare.Application.Features.Profiles.Commands;

// Create Beneficiary
public record CreateBeneficiaryCommand(CreateBeneficiaryRequest Request) : IRequest<Result<BeneficiaryDto>>;

public class CreateBeneficiaryCommandValidator : AbstractValidator<CreateBeneficiaryCommand>
{
    public CreateBeneficiaryCommandValidator()
    {
        RuleFor(x => x.Request.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Request.DateOfBirth).NotEmpty().LessThan(DateTime.UtcNow);
        RuleFor(x => x.Request.Gender).IsInEnum();
    }
}

public class CreateBeneficiaryCommandHandler : IRequestHandler<CreateBeneficiaryCommand, Result<BeneficiaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateBeneficiaryCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BeneficiaryDto>> Handle(CreateBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        // Get current customer
        var user = await _unitOfWork.Users.GetByIdAsync(_currentUserService.UserId!.Value, cancellationToken);
        if (user == null)
            return Result<BeneficiaryDto>.Failure("User not found");

        var customer = await _unitOfWork.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id, cancellationToken);
        if (customer == null)
            return Result<BeneficiaryDto>.Failure("Customer profile not found");

        // Create beneficiary
        var beneficiary = new Beneficiary
        {
            CustomerId = customer.Id,
            FullName = request.Request.FullName,
            DateOfBirth = request.Request.DateOfBirth,
            Gender = request.Request.Gender,
            Address = request.Request.Address,
            MedicalConditions = request.Request.MedicalConditions,
            Medications = request.Request.Medications,
            Allergies = request.Request.Allergies,
            MobilityLevel = request.Request.MobilityLevel,
            CognitiveStatus = request.Request.CognitiveStatus,
            SpecialNeeds = request.Request.SpecialNeeds,
            PersonalityTraits = request.Request.PersonalityTraits,
            Hobbies = request.Request.Hobbies
        };

        await _unitOfWork.Beneficiaries.AddAsync(beneficiary, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

        return Result<BeneficiaryDto>.Success(dto, "Beneficiary created successfully");
    }
}

// Update Beneficiary
public record UpdateBeneficiaryCommand(Guid Id, UpdateBeneficiaryRequest Request) : IRequest<Result<BeneficiaryDto>>;

public class UpdateBeneficiaryCommandValidator : AbstractValidator<UpdateBeneficiaryCommand>
{
    public UpdateBeneficiaryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Request.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Request.DateOfBirth).NotEmpty().LessThan(DateTime.UtcNow);
        RuleFor(x => x.Request.Gender).IsInEnum();
    }
}

public class UpdateBeneficiaryCommandHandler : IRequestHandler<UpdateBeneficiaryCommand, Result<BeneficiaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateBeneficiaryCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BeneficiaryDto>> Handle(UpdateBeneficiaryCommand request, CancellationToken cancellationToken)
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

        // Update beneficiary
        beneficiary.FullName = request.Request.FullName;
        beneficiary.DateOfBirth = request.Request.DateOfBirth;
        beneficiary.Gender = request.Request.Gender;
        beneficiary.Address = request.Request.Address;
        beneficiary.MedicalConditions = request.Request.MedicalConditions;
        beneficiary.Medications = request.Request.Medications;
        beneficiary.Allergies = request.Request.Allergies;
        beneficiary.MobilityLevel = request.Request.MobilityLevel;
        beneficiary.CognitiveStatus = request.Request.CognitiveStatus;
        beneficiary.SpecialNeeds = request.Request.SpecialNeeds;
        beneficiary.PersonalityTraits = request.Request.PersonalityTraits;
        beneficiary.Hobbies = request.Request.Hobbies;

        await _unitOfWork.Beneficiaries.UpdateAsync(beneficiary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

        return Result<BeneficiaryDto>.Success(dto, "Beneficiary updated successfully");
    }
}

// Delete Beneficiary
public record DeleteBeneficiaryCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteBeneficiaryCommandHandler : IRequestHandler<DeleteBeneficiaryCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteBeneficiaryCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(DeleteBeneficiaryCommand request, CancellationToken cancellationToken)
    {
        // Get current customer
        var user = await _unitOfWork.Users.GetByIdAsync(_currentUserService.UserId!.Value, cancellationToken);
        if (user == null)
            return Result<bool>.Failure("User not found");

        var customer = await _unitOfWork.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id, cancellationToken);
        if (customer == null)
            return Result<bool>.Failure("Customer profile not found");

        // Get beneficiary
        var beneficiary = await _unitOfWork.Beneficiaries.GetByIdAsync(request.Id, cancellationToken);
        if (beneficiary == null || beneficiary.CustomerId != customer.Id)
            return Result<bool>.Failure("Beneficiary not found or does not belong to you");

        // Soft delete
        beneficiary.IsDeleted = true;
        await _unitOfWork.Beneficiaries.UpdateAsync(beneficiary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Beneficiary deleted successfully");
    }
}

// Update Beneficiary Preferences
public record UpdateBeneficiaryPreferencesCommand(Guid BeneficiaryId, UpdateBeneficiaryPreferencesRequest Request) 
    : IRequest<Result<BeneficiaryPreferenceDto>>;

public class UpdateBeneficiaryPreferencesCommandHandler 
    : IRequestHandler<UpdateBeneficiaryPreferencesCommand, Result<BeneficiaryPreferenceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateBeneficiaryPreferencesCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<BeneficiaryPreferenceDto>> Handle(
        UpdateBeneficiaryPreferencesCommand request, 
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

        // Get or create preferences
        var preferences = await _unitOfWork.BeneficiaryPreferences.FirstOrDefaultAsync(
            p => p.BeneficiaryId == request.BeneficiaryId, 
            cancellationToken);

        if (preferences == null)
        {
            preferences = new BeneficiaryPreference
            {
                BeneficiaryId = request.BeneficiaryId
            };
            await _unitOfWork.BeneficiaryPreferences.AddAsync(preferences, cancellationToken);
        }

        // Update preferences
        preferences.PreferredGender = request.Request.PreferredGender;
        preferences.PreferredAgeRange = request.Request.PreferredAgeRange;
        preferences.PreferredPersonalityTraits = request.Request.PreferredPersonalityTraits;
        preferences.AvoidPersonalityTraits = request.Request.AvoidPersonalityTraits;
        preferences.SpecialRequirements = request.Request.SpecialRequirements;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

        return Result<BeneficiaryPreferenceDto>.Success(dto, "Preferences updated successfully");
    }
}
