using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Admin.DTOs;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElderCare.Application.Features.Admin.Commands;

// Get Pending Caregivers Query
public record GetPendingCaregiversQuery : IRequest<Result<List<CaregiverApprovalDto>>>;

public class GetPendingCaregiversQueryHandler : IRequestHandler<GetPendingCaregiversQuery, Result<List<CaregiverApprovalDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingCaregiversQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<CaregiverApprovalDto>>> Handle(GetPendingCaregiversQuery request, CancellationToken cancellationToken)
    {
        var caregivers = await _unitOfWork.Caregivers.GetPendingCaregiversAsync(cancellationToken);

        var dtos = caregivers.Select(c => new CaregiverApprovalDto
        {
            Id = c.Id,
            UserId = c.UserId,
            FullName = c.FullName,
            Email = c.User.Email,
            IdentityNumber = c.IdentityNumber,
            IdentityImageUrl = c.IdentityImageUrl,
            SelfieUrl = c.SelfieUrl,
            CriminalRecordUrl = c.CriminalRecordUrl,
            VerificationStatus = c.VerificationStatus,
            CreatedAt = c.CreatedAt
        }).ToList();

        return Result<List<CaregiverApprovalDto>>.Success(dtos, "Pending caregivers retrieved successfully");
    }
}

// Approve Caregiver Command
public record ApproveCaregiverCommand(Guid CaregiverId) : IRequest<Result<CaregiverApprovalDto>>;

public class ApproveCaregiverCommandHandler : IRequestHandler<ApproveCaregiverCommand, Result<CaregiverApprovalDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ApproveCaregiverCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CaregiverApprovalDto>> Handle(ApproveCaregiverCommand request, CancellationToken cancellationToken)
    {
        var caregiver = await _unitOfWork.Caregivers.Query()
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == request.CaregiverId, cancellationToken);

        if (caregiver == null)
            return Result<CaregiverApprovalDto>.Failure("Not found", "Caregiver not found");

        caregiver.VerificationStatus = VerificationStatus.Approved;
        caregiver.ApprovedAt = DateTime.UtcNow;
        caregiver.ApprovedBy = _currentUserService.Email;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new CaregiverApprovalDto
        {
            Id = caregiver.Id,
            UserId = caregiver.UserId,
            FullName = caregiver.FullName,
            Email = caregiver.User.Email,
            IdentityNumber = caregiver.IdentityNumber,
            IdentityImageUrl = caregiver.IdentityImageUrl,
            SelfieUrl = caregiver.SelfieUrl,
            CriminalRecordUrl = caregiver.CriminalRecordUrl,
            VerificationStatus = caregiver.VerificationStatus,
            CreatedAt = caregiver.CreatedAt
        };

        return Result<CaregiverApprovalDto>.Success(dto, "Caregiver approved successfully");
    }
}

// Reject Caregiver Command
public record RejectCaregiverCommand(ApproveRejectRequest Request) : IRequest<Result<CaregiverApprovalDto>>;

public class RejectCaregiverCommandHandler : IRequestHandler<RejectCaregiverCommand, Result<CaregiverApprovalDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RejectCaregiverCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CaregiverApprovalDto>> Handle(RejectCaregiverCommand request, CancellationToken cancellationToken)
    {
        var caregiver = await _unitOfWork.Caregivers.Query()
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == request.Request.CaregiverId, cancellationToken);

        if (caregiver == null)
            return Result<CaregiverApprovalDto>.Failure("Not found", "Caregiver not found");

        caregiver.VerificationStatus = VerificationStatus.Rejected;
        caregiver.RejectionReason = request.Request.RejectionReason;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new CaregiverApprovalDto
        {
            Id = caregiver.Id,
            UserId = caregiver.UserId,
            FullName = caregiver.FullName,
            Email = caregiver.User.Email,
            IdentityNumber = caregiver.IdentityNumber,
            IdentityImageUrl = caregiver.IdentityImageUrl,
            SelfieUrl = caregiver.SelfieUrl,
            CriminalRecordUrl = caregiver.CriminalRecordUrl,
            VerificationStatus = caregiver.VerificationStatus,
            CreatedAt = caregiver.CreatedAt
        };

        return Result<CaregiverApprovalDto>.Success(dto, "Caregiver rejected");
    }
}
