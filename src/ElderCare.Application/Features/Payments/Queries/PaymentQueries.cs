using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Payments.DTOs;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ElderCare.Application.Features.Payments.Queries;

// ============================================
// 1. Get Wallet Balance Query
// ============================================
public record GetWalletBalanceQuery : IRequest<Result<WalletDto>>;

public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, Result<WalletDto>>
{
    private readonly IRepository<Wallet> _walletRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetWalletBalanceQueryHandler(
        IRepository<Wallet> walletRepo,
        IHttpContextAccessor httpContextAccessor)
    {
        _walletRepo = walletRepo;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<WalletDto>> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
    {
        // Get current user ID
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Result<WalletDto>.Failure("User not authenticated");

        // Get user's wallet
        var wallet = await _walletRepo.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
            return Result<WalletDto>.Failure("Wallet not found");

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
// 2. Get Transaction History Query
// ============================================
public record GetTransactionHistoryQuery(
    int PageNumber = 1,
    int PageSize = 20,
    TransactionType? FilterType = null
) : IRequest<Result<PaginatedList<TransactionDto>>>;

public class GetTransactionHistoryQueryHandler : IRequestHandler<GetTransactionHistoryQuery, Result<PaginatedList<TransactionDto>>>
{
    private readonly IRepository<Wallet> _walletRepo;
    private readonly IRepository<Transaction> _transactionRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetTransactionHistoryQueryHandler(
        IRepository<Wallet> walletRepo,
        IRepository<Transaction> transactionRepo,
        IHttpContextAccessor httpContextAccessor)
    {
        _walletRepo = walletRepo;
        _transactionRepo = transactionRepo;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<PaginatedList<TransactionDto>>> Handle(GetTransactionHistoryQuery request, CancellationToken cancellationToken)
    {
        // Get current user ID
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Result<PaginatedList<TransactionDto>>.Failure("User not authenticated");

        // Get user's wallet
        var wallet = await _walletRepo.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet == null)
            return Result<PaginatedList<TransactionDto>>.Failure("Wallet not found");

        // Get transactions with optional filter
        var allTransactions = await _transactionRepo.GetAllAsync(t => t.WalletId == wallet.Id);
        var transactions = allTransactions.ToList();

        // Apply type filter if specified
        if (request.FilterType.HasValue)
        {
            transactions = transactions.Where(t => t.Type == request.FilterType.Value).ToList();
        }

        // Order by created date descending
        var orderedTransactions = transactions.OrderByDescending(t => t.CreatedAt).ToList();

        // Apply pagination
        var totalCount = orderedTransactions.Count;
        var items = orderedTransactions
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                WalletId = t.WalletId,
                TransactionType = t.Type.ToString(),
                Amount = t.Amount,
                Description = t.Description,
                Status = t.Status.ToString(),
                CreatedAt = t.CreatedAt,
                RelatedBookingId = t.RelatedBookingId
            })
            .ToList();

        var paginatedList = new PaginatedList<TransactionDto>(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        return Result<PaginatedList<TransactionDto>>.Success(paginatedList);
    }
}
