using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Common.Models;
using ElderCare.Application.Features.Tracking.DTOs;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ElderCare.Application.Features.Tracking.Commands;

// ============================================
// 1. Update Location Command
// ============================================
public record UpdateLocationCommand(
    Guid BookingId,
    double Latitude,
    double Longitude,
    double? Accuracy = null
) : IRequest<Result<LocationDto>>;

public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, Result<LocationDto>>
{
    private readonly ILocationService _locationService;
    private readonly IRepository<Booking> _bookingRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateLocationCommandHandler(
        ILocationService locationService,
        IRepository<Booking> bookingRepo,
        IHttpContextAccessor httpContextAccessor)
    {
        _locationService = locationService;
        _bookingRepo = bookingRepo;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<LocationDto>> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        // Get booking
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null)
            return Result<LocationDto>.Failure("Booking not found");

        // Validate booking is in progress
        if (booking.Status != BookingStatus.InProgress)
            return Result<LocationDto>.Failure("Booking is not in progress. Location tracking is only available during active service.");

        // Get current user ID
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Result<LocationDto>.Failure("User not authenticated");

        // TODO: Verify user is the caregiver for this booking
        // This requires accessing User -> CaregiverProfile relationship
        // For now, we'll skip this check

        // Validate GPS accuracy (reject if too inaccurate)
        if (request.Accuracy.HasValue && request.Accuracy.Value > 50)
            return Result<LocationDto>.Failure($"GPS accuracy too low ({request.Accuracy:F0}m). Please ensure GPS is enabled and try again.");

        // Log location
        var locationLog = await _locationService.LogLocationAsync(
            request.BookingId,
            request.Latitude,
            request.Longitude,
            request.Accuracy
        );

        // Calculate distance from service location
        var distance = await _locationService.CalculateDistanceAsync(
            request.Latitude,
            request.Longitude,
            booking.Latitude,
            booking.Longitude
        );

        var locationDto = new LocationDto
        {
            Id = locationLog.Id,
            BookingId = locationLog.BookingId,
            Latitude = locationLog.Latitude,
            Longitude = locationLog.Longitude,
            Accuracy = locationLog.Accuracy,
            Timestamp = locationLog.Timestamp,
            DistanceFromServiceLocation = distance
        };

        return Result<LocationDto>.Success(locationDto);
    }
}

// ============================================
// 2. Validate Geofence Command
// ============================================
public record ValidateGeofenceCommand(
    Guid BookingId,
    double CurrentLatitude,
    double CurrentLongitude
) : IRequest<Result<GeofenceValidationDto>>;

public class ValidateGeofenceCommandHandler : IRequestHandler<ValidateGeofenceCommand, Result<GeofenceValidationDto>>
{
    private readonly ILocationService _locationService;
    private readonly IRepository<Booking> _bookingRepo;

    public ValidateGeofenceCommandHandler(
        ILocationService locationService,
        IRepository<Booking> bookingRepo)
    {
        _locationService = locationService;
        _bookingRepo = bookingRepo;
    }

    public async Task<Result<GeofenceValidationDto>> Handle(ValidateGeofenceCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null)
            return Result<GeofenceValidationDto>.Failure("Booking not found");

        var distance = await _locationService.CalculateDistanceAsync(
            request.CurrentLatitude,
            request.CurrentLongitude,
            booking.Latitude,
            booking.Longitude
        );

        var isWithinGeofence = await _locationService.IsWithinGeofenceAsync(
            request.CurrentLatitude,
            request.CurrentLongitude,
            booking.Latitude,
            booking.Longitude,
            booking.GeofenceRadiusMeters
        );

        var message = isWithinGeofence
            ? $"Location verified. You are {distance:F0}m from the service location."
            : $"You are outside the geofence ({distance:F0}m away). Please move within {booking.GeofenceRadiusMeters}m of the service location.";

        var result = new GeofenceValidationDto
        {
            BookingId = booking.Id,
            IsWithinGeofence = isWithinGeofence,
            Distance = distance,
            GeofenceRadius = booking.GeofenceRadiusMeters,
            ServiceLocation = new LocationPointDto
            {
                Latitude = booking.Latitude,
                Longitude = booking.Longitude
            },
            CurrentLocation = new LocationPointDto
            {
                Latitude = request.CurrentLatitude,
                Longitude = request.CurrentLongitude
            },
            Message = message
        };

        return Result<GeofenceValidationDto>.Success(result);
    }
}
