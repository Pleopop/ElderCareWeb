using ElderCare.Application.Common.Models;
using MediatR;

namespace ElderCare.Application.Features.Payments.DTOs;

public class WalletDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
    public decimal EscrowBalance { get; set; }
    public decimal TotalBalance => Balance + EscrowBalance;
    public bool IsActive { get; set; }
}

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid? RelatedBookingId { get; set; }
}

public class EscrowDetailsDto
{
    public Guid BookingId { get; set; }
    public decimal EscrowAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal CaregiverAmount { get; set; }
    public DateTime HeldAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

// Request DTOs
public class DepositRequest
{
    public decimal Amount { get; set; }
}

public class HoldEscrowRequest
{
    public Guid BookingId { get; set; }
}

public class ReleaseEscrowRequest
{
    public Guid BookingId { get; set; }
}

public class RefundEscrowRequest
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
