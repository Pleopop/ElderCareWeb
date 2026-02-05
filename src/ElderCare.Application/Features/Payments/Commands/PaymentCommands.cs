using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Payments.DTOs;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ElderCare.Application.Features.Payments.Commands;

// ============================================
// 1. Deposit to Wallet Command
// ============================================
public record DepositToWalletCommand(decimal Amount) : IRequest<Result<WalletDto>>;

public class DepositToWalletCommandHandler : IRequestHandler<DepositToWalletCommand, Result<WalletDto>>
{
    private readonly IRepository<Wallet> _walletRepo;
    private readonly IRepository<Transaction> _transactionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DepositToWalletCommandHandler(
        IRepository<Wallet> walletRepo,
        IRepository<Transaction> transactionRepo,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor)
    {
        _walletRepo = walletRepo;
        _transactionRepo = transactionRepo;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<WalletDto>> Handle(DepositToWalletCommand request, CancellationToken cancellationToken)
    {
        // Validate amount
        if (request.Amount <= 0)
            return Result<WalletDto>.Failure("Deposit amount must be greater than zero");

        // Get current user ID
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Result<WalletDto>.Failure("User not authenticated");

        // Get user's wallet
        var wallet = await _walletRepo.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
            return Result<WalletDto>.Failure("Wallet not found");

        if (!wallet.IsActive)
            return Result<WalletDto>.Failure("Wallet is not active");

        // Create deposit transaction
        var transaction = new Transaction
        {
            WalletId = wallet.Id,
            Type = TransactionType.Deposit,
            Amount = request.Amount,
            Description = $"Deposit to wallet",
            Status = TransactionStatus.Completed
        };

        await _transactionRepo.AddAsync(transaction);

        // Update wallet balance
        wallet.Balance += request.Amount;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return updated wallet
        var walletDto = new WalletDto
        {
            Id = wallet.Id,
            UserId = wallet.UserId,
            Balance = wallet.Balance,
            EscrowBalance = wallet.EscrowBalance,
            IsActive = wallet.IsActive
        };

        return Result<WalletDto>.Success(walletDto);
    }
}

// ============================================
// 2. Hold Escrow Command
// ============================================
public record HoldEscrowCommand(Guid BookingId) : IRequest<Result<TransactionDto>>;

public class HoldEscrowCommandHandler : IRequestHandler<HoldEscrowCommand, Result<TransactionDto>>
{
    private readonly IRepository<Booking> _bookingRepo;
    private readonly IRepository<Wallet> _walletRepo;
    private readonly IRepository<Transaction> _transactionRepo;
    private readonly IUnitOfWork _unitOfWork;

    public HoldEscrowCommandHandler(
        IRepository<Booking> bookingRepo,
        IRepository<Wallet> walletRepo,
        IRepository<Transaction> transactionRepo,
        IUnitOfWork unitOfWork)
    {
        _bookingRepo = bookingRepo;
        _walletRepo = walletRepo;
        _transactionRepo = transactionRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TransactionDto>> Handle(HoldEscrowCommand request, CancellationToken cancellationToken)
    {
        // Get booking
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null)
            return Result<TransactionDto>.Failure("Booking not found");

        // Check if escrow already held
        if (booking.EscrowTransactionId.HasValue)
            return Result<TransactionDto>.Failure("Escrow already held for this booking");

        // Get customer's wallet
        var customerWallet = await _walletRepo.FirstOrDefaultAsync(w => 
            w.User.CustomerProfile != null && w.User.CustomerProfile.Id == booking.CustomerProfileId);
        
        if (customerWallet == null)
            return Result<TransactionDto>.Failure("Customer wallet not found");

        var escrowAmount = booking.TotalAmount;

        // Check sufficient balance
        if (customerWallet.Balance < escrowAmount)
            return Result<TransactionDto>.Failure($"Insufficient balance. Required: {escrowAmount:N0}, Available: {customerWallet.Balance:N0}");

        // Create escrow hold transaction
        var transaction = new Transaction
        {
            WalletId = customerWallet.Id,
            Type = TransactionType.EscrowHold,
            Amount = escrowAmount,
            Description = $"Escrow hold for booking #{booking.Id.ToString().Substring(0, 8)}",
            Status = TransactionStatus.Completed,
            RelatedBookingId = booking.Id
        };

        await _transactionRepo.AddAsync(transaction);

        // Update customer wallet
        customerWallet.Balance -= escrowAmount;
        customerWallet.EscrowBalance += escrowAmount;

        // Update booking
        booking.EscrowAmount = escrowAmount;
        booking.EscrowHeldAt = DateTime.UtcNow;
        booking.EscrowTransactionId = transaction.Id;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return transaction DTO
        var transactionDto = new TransactionDto
        {
            Id = transaction.Id,
            WalletId = transaction.WalletId,
            TransactionType = transaction.Type.ToString(),
            Amount = transaction.Amount,
            Description = transaction.Description,
            Status = transaction.Status.ToString(),
            CreatedAt = transaction.CreatedAt,
            RelatedBookingId = transaction.RelatedBookingId
        };

        return Result<TransactionDto>.Success(transactionDto);
    }
}

// ============================================
// 3. Release Escrow Command
// ============================================
public record ReleaseEscrowCommand(Guid BookingId) : IRequest<Result<List<TransactionDto>>>;

public class ReleaseEscrowCommandHandler : IRequestHandler<ReleaseEscrowCommand, Result<List<TransactionDto>>>
{
    private readonly IRepository<Booking> _bookingRepo;
    private readonly IRepository<Wallet> _walletRepo;
    private readonly IRepository<Transaction> _transactionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private const decimal COMMISSION_RATE = 0.15m; // 15%

    public ReleaseEscrowCommandHandler(
        IRepository<Booking> bookingRepo,
        IRepository<Wallet> walletRepo,
        IRepository<Transaction> transactionRepo,
        IUnitOfWork unitOfWork)
    {
        _bookingRepo = bookingRepo;
        _walletRepo = walletRepo;
        _transactionRepo = transactionRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<TransactionDto>>> Handle(ReleaseEscrowCommand request, CancellationToken cancellationToken)
    {
        // Get booking
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null)
            return Result<List<TransactionDto>>.Failure("Booking not found");

        // Validate booking status
        if (booking.Status != BookingStatus.Completed)
            return Result<List<TransactionDto>>.Failure("Booking must be completed before releasing escrow");

        // Check if escrow already released
        if (booking.EscrowReleasedAt.HasValue)
            return Result<List<TransactionDto>>.Failure("Escrow already released for this booking");

        // Check if escrow was held
        if (!booking.EscrowAmount.HasValue || !booking.EscrowTransactionId.HasValue)
            return Result<List<TransactionDto>>.Failure("No escrow found for this booking");

        var escrowAmount = booking.EscrowAmount.Value;
        var commissionAmount = escrowAmount * COMMISSION_RATE;
        var caregiverAmount = escrowAmount - commissionAmount;

        // Get wallets
        var customerWallet = await _walletRepo.FirstOrDefaultAsync(w => 
            w.User.CustomerProfile != null && w.User.CustomerProfile.Id == booking.CustomerProfileId);
        
        var caregiverWallet = await _walletRepo.FirstOrDefaultAsync(w => 
            w.User.CaregiverProfile != null && w.User.CaregiverProfile.Id == booking.CaregiverProfileId);

        if (customerWallet == null || caregiverWallet == null)
            return Result<List<TransactionDto>>.Failure("Wallet not found");

        var transactions = new List<Transaction>();

        // 1. Commission transaction (deduct from escrow)
        var commissionTx = new Transaction
        {
            WalletId = customerWallet.Id,
            Type = TransactionType.Commission,
            Amount = commissionAmount,
            Description = $"Platform commission (15%) for booking #{booking.Id.ToString().Substring(0, 8)}",
            Status = TransactionStatus.Completed,
            RelatedBookingId = booking.Id
        };
        transactions.Add(commissionTx);

        // 2. Release transaction (to caregiver)
        var releaseTx = new Transaction
        {
            WalletId = caregiverWallet.Id,
            Type = TransactionType.EscrowRelease,
            Amount = caregiverAmount,
            Description = $"Payment for booking #{booking.Id.ToString().Substring(0, 8)}",
            Status = TransactionStatus.Completed,
            RelatedBookingId = booking.Id
        };
        transactions.Add(releaseTx);

        await _transactionRepo.AddRangeAsync(transactions);

        // Update customer wallet (reduce escrow)
        customerWallet.EscrowBalance -= escrowAmount;

        // Update caregiver wallet (add payment)
        caregiverWallet.Balance += caregiverAmount;

        // Update booking
        booking.EscrowReleasedAt = DateTime.UtcNow;
        booking.CommissionAmount = commissionAmount;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return transaction DTOs
        var transactionDtos = transactions.Select(t => new TransactionDto
        {
            Id = t.Id,
            WalletId = t.WalletId,
            TransactionType = t.Type.ToString(),
            Amount = t.Amount,
            Description = t.Description,
            Status = t.Status.ToString(),
            CreatedAt = t.CreatedAt,
            RelatedBookingId = t.RelatedBookingId
        }).ToList();

        return Result<List<TransactionDto>>.Success(transactionDtos);
    }
}

// ============================================
// 4. Refund Escrow Command
// ============================================
public record RefundEscrowCommand(Guid BookingId, string Reason) : IRequest<Result<TransactionDto>>;

public class RefundEscrowCommandHandler : IRequestHandler<RefundEscrowCommand, Result<TransactionDto>>
{
    private readonly IRepository<Booking> _bookingRepo;
    private readonly IRepository<Wallet> _walletRepo;
    private readonly IRepository<Transaction> _transactionRepo;
    private readonly IUnitOfWork _unitOfWork;

    public RefundEscrowCommandHandler(
        IRepository<Booking> bookingRepo,
        IRepository<Wallet> walletRepo,
        IRepository<Transaction> transactionRepo,
        IUnitOfWork unitOfWork)
    {
        _bookingRepo = bookingRepo;
        _walletRepo = walletRepo;
        _transactionRepo = transactionRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TransactionDto>> Handle(RefundEscrowCommand request, CancellationToken cancellationToken)
    {
        // Get booking
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null)
            return Result<TransactionDto>.Failure("Booking not found");

        // Check if escrow was held
        if (!booking.EscrowAmount.HasValue || !booking.EscrowTransactionId.HasValue)
            return Result<TransactionDto>.Failure("No escrow found for this booking");

        // Check if already refunded or released
        if (booking.EscrowReleasedAt.HasValue)
            return Result<TransactionDto>.Failure("Escrow already processed for this booking");

        // Validate booking can be refunded
        if (booking.Status == BookingStatus.Completed)
            return Result<TransactionDto>.Failure("Cannot refund completed booking");

        var refundAmount = booking.EscrowAmount.Value;

        // Get customer wallet
        var customerWallet = await _walletRepo.FirstOrDefaultAsync(w => 
            w.User.CustomerProfile != null && w.User.CustomerProfile.Id == booking.CustomerProfileId);

        if (customerWallet == null)
            return Result<TransactionDto>.Failure("Customer wallet not found");

        // Create refund transaction
        var transaction = new Transaction
        {
            WalletId = customerWallet.Id,
            Type = TransactionType.Refund,
            Amount = refundAmount,
            Description = $"Refund for cancelled booking #{booking.Id.ToString().Substring(0, 8)}: {request.Reason}",
            Status = TransactionStatus.Completed,
            RelatedBookingId = booking.Id
        };

        await _transactionRepo.AddAsync(transaction);

        // Update customer wallet (return funds)
        customerWallet.EscrowBalance -= refundAmount;
        customerWallet.Balance += refundAmount;

        // Update booking
        booking.Status = BookingStatus.Cancelled;
        booking.CancellationReason = request.Reason;
        booking.EscrowReleasedAt = DateTime.UtcNow; // Mark as processed

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return transaction DTO
        var transactionDto = new TransactionDto
        {
            Id = transaction.Id,
            WalletId = transaction.WalletId,
            TransactionType = transaction.Type.ToString(),
            Amount = transaction.Amount,
            Description = transaction.Description,
            Status = transaction.Status.ToString(),
            CreatedAt = transaction.CreatedAt,
            RelatedBookingId = transaction.RelatedBookingId
        };

        return Result<TransactionDto>.Success(transactionDto);
    }
}
