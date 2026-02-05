using ElderCare.Application.Common.Interfaces;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Interfaces;

namespace ElderCare.Application.Services;

public class LocationService : ILocationService
{
    private readonly IRepository<LocationLog> _locationLogRepo;
    private readonly IRepository<Booking> _bookingRepo;
    private readonly IUnitOfWork _unitOfWork;

    public LocationService(
        IRepository<LocationLog> locationLogRepo,
        IRepository<Booking> bookingRepo,
        IUnitOfWork unitOfWork)
    {
        _locationLogRepo = locationLogRepo;
        _bookingRepo = bookingRepo;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Calculate distance between two GPS coordinates using Haversine formula
    /// </summary>
    /// <returns>Distance in meters</returns>
    public Task<double> CalculateDistanceAsync(double lat1, double lng1, double lat2, double lng2)
    {
        const double R = 6371000; // Earth radius in meters
        
        var dLat = ToRadians(lat2 - lat1);
        var dLng = ToRadians(lng2 - lng1);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        var distance = R * c; // Distance in meters
        
        return Task.FromResult(distance);
    }

    /// <summary>
    /// Check if current location is within geofence radius
    /// </summary>
    public async Task<bool> IsWithinGeofenceAsync(
        double currentLat, 
        double currentLng, 
        double targetLat, 
        double targetLng, 
        int radiusMeters)
    {
        var distance = await CalculateDistanceAsync(currentLat, currentLng, targetLat, targetLng);
        return distance <= radiusMeters;
    }

    /// <summary>
    /// Log location update for a booking
    /// </summary>
    public async Task<LocationLog> LogLocationAsync(
        Guid bookingId, 
        double latitude, 
        double longitude, 
        double? accuracy = null)
    {
        // Verify booking exists
        var booking = await _bookingRepo.GetByIdAsync(bookingId);
        if (booking == null)
            throw new InvalidOperationException("Booking not found");

        var locationLog = new LocationLog
        {
            BookingId = bookingId,
            Latitude = latitude,
            Longitude = longitude,
            Accuracy = accuracy,
            Timestamp = DateTime.UtcNow
        };

        await _locationLogRepo.AddAsync(locationLog);
        await _unitOfWork.SaveChangesAsync();

        return locationLog;
    }

    /// <summary>
    /// Get location history for a booking
    /// </summary>
    public async Task<List<LocationLog>> GetLocationHistoryAsync(
        Guid bookingId, 
        DateTime? from = null, 
        DateTime? to = null)
    {
        var allLogs = await _locationLogRepo.GetAllAsync(l => l.BookingId == bookingId);
        var logs = allLogs.ToList();

        // Apply date filters if provided
        if (from.HasValue)
        {
            logs = logs.Where(l => l.Timestamp >= from.Value).ToList();
        }

        if (to.HasValue)
        {
            logs = logs.Where(l => l.Timestamp <= to.Value).ToList();
        }

        // Order by timestamp ascending (oldest first)
        return logs.OrderBy(l => l.Timestamp).ToList();
    }

    /// <summary>
    /// Get most recent location for a booking
    /// </summary>
    public async Task<LocationLog?> GetCurrentLocationAsync(Guid bookingId)
    {
        var allLogs = await _locationLogRepo.GetAllAsync(l => l.BookingId == bookingId);
        var logs = allLogs.ToList();

        // Return most recent location
        return logs.OrderByDescending(l => l.Timestamp).FirstOrDefault();
    }

    /// <summary>
    /// Convert degrees to radians
    /// </summary>
    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
