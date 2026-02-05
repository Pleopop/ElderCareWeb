using ElderCare.Application.Features.Admin.Commands;
using ElderCare.Application.Features.Admin.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElderCare.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/v1/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("caregivers/pending")]
    public async Task<IActionResult> GetPendingCaregivers()
    {
        var query = new GetPendingCaregiversQuery();
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("caregivers/{id}/approve")]
    public async Task<IActionResult> ApproveCaregiver(Guid id)
    {
        var command = new ApproveCaregiverCommand(id);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("caregivers/reject")]
    public async Task<IActionResult> RejectCaregiver([FromBody] ApproveRejectRequest request)
    {
        var command = new RejectCaregiverCommand(request);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
