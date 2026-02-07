using ElderCare.Domain.Enums;

namespace ElderCare.Application.Features.Admin.DTOs;

public class CaregiverApprovalDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? IdentityNumber { get; set; }
    public string? IdentityImageUrl { get; set; }
    public string? SelfieUrl { get; set; }
    public string? CriminalRecordUrl { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ApproveRejectRequest
{
    public Guid CaregiverId { get; set; }
    public string? RejectionReason { get; set; }
}
