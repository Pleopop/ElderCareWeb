using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Payments.Commands;
using ElderCare.Application.Features.Payments.DTOs;
using ElderCare.Application.Features.Payments.Queries;
using ElderCare.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElderCare.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Deposit money to wallet
    /// </summary>
    [HttpPost("deposit")]
    public async Task<ActionResult<Result<WalletDto>>> DepositToWallet([FromBody] DepositRequest request)
    {
        var command = new DepositToWalletCommand(request.Amount);
        var result = await _mediator.Send(command);
        
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Hold escrow for a booking (called automatically when booking is created)
    /// </summary>
    [HttpPost("escrow/hold")]
    public async Task<ActionResult<Result<TransactionDto>>> HoldEscrow([FromBody] HoldEscrowRequest request)
    {
        var command = new HoldEscrowCommand(request.BookingId);
        var result = await _mediator.Send(command);
        
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Release escrow to caregiver after booking completion
    /// </summary>
    [HttpPost("escrow/release")]
    public async Task<ActionResult<Result<List<TransactionDto>>>> ReleaseEscrow([FromBody] ReleaseEscrowRequest request)
    {
        var command = new ReleaseEscrowCommand(request.BookingId);
        var result = await _mediator.Send(command);
        
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Refund escrow to customer on booking cancellation
    /// </summary>
    [HttpPost("escrow/refund")]
    public async Task<ActionResult<Result<TransactionDto>>> RefundEscrow([FromBody] RefundEscrowRequest request)
    {
        var command = new RefundEscrowCommand(request.BookingId, request.Reason);
        var result = await _mediator.Send(command);
        
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get current user's wallet balance
    /// </summary>
    [HttpGet("wallet")]
    public async Task<ActionResult<Result<WalletDto>>> GetWalletBalance()
    {
        var query = new GetWalletBalanceQuery();
        var result = await _mediator.Send(query);
        
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get transaction history with pagination and optional filtering
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<Result<PaginatedList<TransactionDto>>>> GetTransactionHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TransactionType? filterType = null)
    {
        var query = new GetTransactionHistoryQuery(pageNumber, pageSize, filterType);
        var result = await _mediator.Send(query);
        
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
