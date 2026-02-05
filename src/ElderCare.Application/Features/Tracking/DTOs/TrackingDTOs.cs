using ElderCare.Application.Common.Models;
using MediatR;

namespace ElderCare.Application.Features.Tracking.DTOs;

public class LocationDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Accuracy { get; set; }
    public DateTime Timestamp { get; set; }
    public double? DistanceFromServiceLocation { get; set; }
}

public class LocationPointDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class GeofenceValidationDto
{
    public Guid BookingId { get; set; }
    public bool IsWithinGeofence { get; set; }
    public double Distance { get; set; }
    public int GeofenceRadius { get; set; }
    public LocationPointDto ServiceLocation { get; set; } = null!;
    public LocationPointDto CurrentLocation { get; set; } = null!;
    public string Message { get; set; } = string.Empty;
}

// Request DTOs
public class UpdateLocationRequest
{
    public Guid BookingId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Accuracy { get; set; }
}

public class ValidateGeofenceRequest
{
    public Guid BookingId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
