using ElderCare.Application.Features.Matching.Commands;
using ElderCare.Application.Features.Matching.DTOs;
using ElderCare.Application.Features.Matching.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElderCare.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class MatchingController : ControllerBase
{
    private readonly IMediator _mediator;

    public MatchingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Calculate matches for a beneficiary
    /// </summary>
    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateMatches([FromBody] CalculateMatchRequest request)
    {
        var command = new CalculateMatchCommand(request.BeneficiaryId);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Get top matches for a beneficiary
    /// </summary>
    [HttpGet("top-matches/{beneficiaryId}")]
    public async Task<IActionResult> GetTopMatches(Guid beneficiaryId, [FromQuery] int topN = 10)
    {
        var query = new GetTopMatchesQuery(beneficiaryId, topN);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
