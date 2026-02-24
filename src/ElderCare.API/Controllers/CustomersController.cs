using ElderCare.Application.Features.Profiles.Commands;
using ElderCare.Application.Features.Profiles.DTOs;
using ElderCare.Application.Features.Profiles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElderCare.API.Controllers;

[Authorize(Roles = "Customer")]
[ApiController]
[Route("api/v1/customers")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ==================== BENEFICIARY MANAGEMENT ====================

    /// <summary>
    /// Create a new beneficiary
    /// </summary>
    [HttpPost("beneficiaries")]
    public async Task<IActionResult> CreateBeneficiary([FromBody] CreateBeneficiaryRequest request)
    {
        var command = new CreateBeneficiaryCommand(request);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Get all my beneficiaries
    /// </summary>
    [HttpGet("beneficiaries")]
    public async Task<IActionResult> GetMyBeneficiaries()
    {
        var query = new GetMyBeneficiariesQuery();
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Get beneficiary by ID
    /// </summary>
    [HttpGet("beneficiaries/{id}")]
    public async Task<IActionResult> GetBeneficiary(Guid id)
    {
        var query = new GetBeneficiaryByIdQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Update beneficiary
    /// </summary>
    [HttpPut("beneficiaries/{id}")]
    public async Task<IActionResult> UpdateBeneficiary(Guid id, [FromBody] UpdateBeneficiaryRequest request)
    {
        var command = new UpdateBeneficiaryCommand(id, request);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Delete beneficiary
    /// </summary>
    [HttpDelete("beneficiaries/{id}")]
    public async Task<IActionResult> DeleteBeneficiary(Guid id)
    {
        var command = new DeleteBeneficiaryCommand(id);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    // ==================== BENEFICIARY PREFERENCES ====================

    /// <summary>
    /// Update beneficiary preferences for caregiver matching
    /// </summary>
    [HttpPost("beneficiaries/{beneficiaryId}/preferences")]
    public async Task<IActionResult> UpdateBeneficiaryPreferences(
        Guid beneficiaryId, 
        [FromBody] UpdateBeneficiaryPreferencesRequest request)
    {
        var command = new UpdateBeneficiaryPreferencesCommand(beneficiaryId, request);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Get beneficiary preferences
    /// </summary>
    [HttpGet("beneficiaries/{beneficiaryId}/preferences")]
    public async Task<IActionResult> GetBeneficiaryPreferences(Guid beneficiaryId)
    {
        var query = new GetBeneficiaryPreferencesQuery(beneficiaryId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
