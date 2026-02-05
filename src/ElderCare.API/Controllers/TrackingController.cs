using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Tracking.Commands;
using ElderCare.Application.Features.Tracking.DTOs;
using ElderCare.Application.Features.Tracking.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElderCare.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TrackingController : ControllerBase
{
    private readonly IMediator _mediator;

    public TrackingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Update caregiver's current location (called every 30 seconds during service)
    /// </summary>
    [HttpPost("update-location")]
    public async Task<ActionResult<Result<LocationDto>>> UpdateLocation([FromBody] UpdateLocationRequest request)
    {
        var command = new UpdateLocationCommand(
            request.BookingId,
            request.Latitude,
            request.Longitude,
            request.Accuracy
        );
        
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Validate if current location is within geofence (for check-in/check-out)
    /// </summary>
    [HttpPost("validate-geofence")]
    public async Task<ActionResult<Result<GeofenceValidationDto>>> ValidateGeofence([FromBody] ValidateGeofenceRequest request)
    {
        var command = new ValidateGeofenceCommand(
            request.BookingId,
            request.Latitude,
            request.Longitude
        );
        
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get current location of caregiver for a booking
    /// </summary>
    [HttpGet("current/{bookingId}")]
    public async Task<ActionResult<Result<LocationDto>>> GetCurrentLocation(Guid bookingId)
    {
        var query = new GetCurrentLocationQuery(bookingId);
        var result = await _mediator.Send(query);
        
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Get location history for a booking
    /// </summary>
    [HttpGet("history/{bookingId}")]
    public async Task<ActionResult<Result<List<LocationDto>>>> GetLocationHistory(
        Guid bookingId,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var query = new GetLocationHistoryQuery(bookingId, from, to);
        var result = await _mediator.Send(query);
        
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
