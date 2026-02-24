using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ElderCare.Application.Features.FraudDetection.Commands;
using ElderCare.Application.Features.FraudDetection.Queries;

namespace ElderCare.API.Controllers;

/// <summary>
/// Fraud Detection and Monitoring API
/// </summary>
[ApiController]
[Route("api/fraud-detection")]
[Authorize(Roles = "Admin")]
public class FraudDetectionController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public FraudDetectionController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// Get fraud alerts with optional filtering
    /// </summary>
    /// <param name="userId">Optional user ID to filter alerts</param>
    /// <param name="status">Optional status filter (Pending, Investigating, Resolved, FalsePositive)</param>
    [HttpGet("alerts")]
    public async Task<IActionResult> GetAlerts(
        [FromQuery] Guid? userId = null,
        [FromQuery] string? status = null)
    {
        var query = new GetFraudAlertsQuery(userId, status);
        var alerts = await _mediator.Send(query);
        return Ok(alerts);
    }
    
    /// <summary>
    /// Calculate fraud score for a user
    /// </summary>
    [HttpPost("scores/{userId}")]
    public async Task<IActionResult> CalculateFraudScore(Guid userId)
    {
        var command = new CalculateFraudScoreCommand(userId);
        var score = await _mediator.Send(command);
        return Ok(score);
    }
    
    /// <summary>
    /// Get fraud score history for a user
    /// </summary>
    [HttpGet("scores/{userId}/history")]
    public async Task<IActionResult> GetFraudScoreHistory(Guid userId)
    {
        var query = new GetFraudScoreHistoryQuery(userId);
        var scores = await _mediator.Send(query);
        return Ok(scores);
    }
    
    /// <summary>
    /// Resolve a fraud alert
    /// </summary>
    [HttpPost("alerts/{id}/resolve")]
    public async Task<IActionResult> ResolveAlert(
        Guid id,
        [FromBody] ResolveAlertRequest request)
    {
        var command = new ResolveAlertCommand(id, request.Resolution, request.InvestigatedBy);
        await _mediator.Send(command);
        return NoContent();
    }
    
    /// <summary>
    /// Create a fraud alert manually
    /// </summary>
    [HttpPost("alerts")]
    public async Task<IActionResult> CreateAlert([FromBody] CreateAlertRequest request)
    {
        var command = new CreateFraudAlertCommand(
            request.UserId,
            request.AlertType,
            request.Severity,
            request.Description
        );
        var alert = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAlerts), new { userId = alert.UserId }, alert);
    }
    
    /// <summary>
    /// Get suspicious activities
    /// </summary>
    [HttpGet("suspicious-activities")]
    public async Task<IActionResult> GetSuspiciousActivities(
        [FromQuery] Guid? userId = null,
        [FromQuery] int? minRiskScore = null)
    {
        var query = new GetSuspiciousActivitiesQuery(userId, minRiskScore);
        var activities = await _mediator.Send(query);
        return Ok(activities);
    }
}

// DTOs
public record ResolveAlertRequest(string Resolution, string InvestigatedBy);

public record CreateAlertRequest(
    Guid UserId,
    string AlertType,
    int Severity,
    string Description
);
