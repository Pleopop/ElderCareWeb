using System.Security.Claims;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;

namespace ElderCare.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string email, string role);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}

public interface IOtpService
{
    Task<string> GenerateOtpAsync(string identifier);
    Task<bool> ValidateOtpAsync(string identifier, string otp);
}

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder);
    Task<bool> DeleteFileAsync(string fileUrl);
    string GetFileUrl(string fileName, string folder);
}

public interface IMatchingService
{
    Task<MatchingResult> CalculateMatchAsync(
        Guid beneficiaryId,
        Guid caregiverProfileId,
        CaregiverProfile caregiver,
        Beneficiary beneficiary);
}

public interface ILocationService
{
    Task<double> CalculateDistanceAsync(double lat1, double lng1, double lat2, double lng2);
    Task<bool> IsWithinGeofenceAsync(double currentLat, double currentLng, double targetLat, double targetLng, int radiusMeters);
    Task<LocationLog> LogLocationAsync(Guid bookingId, double latitude, double longitude, double? accuracy = null);
    Task<List<LocationLog>> GetLocationHistoryAsync(Guid bookingId, DateTime? from = null, DateTime? to = null);
    Task<LocationLog?> GetCurrentLocationAsync(Guid bookingId);
}

public interface INotificationService
{
    Task<Notification> SendNotificationAsync(Guid userId, string title, string message, string category, string type, string priority, string? actionUrl, Guid? relatedEntityId, string? relatedEntityType);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteNotificationAsync(Guid notificationId);
}
