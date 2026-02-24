using ElderCare.Domain.Entities;

namespace ElderCare.Application.Services;

/// <summary>
/// Service for detecting and managing fraud activities
/// </summary>
public interface IFraudDetectionService
{
    /// <summary>
    /// Calculate overall fraud risk score for a user
    /// </summary>
    Task<FraudScore> CalculateFraudScoreAsync(Guid userId);
    
    /// <summary>
    /// Check for GPS-related fraud (velocity, geofence violations)
    /// </summary>
    Task<bool> CheckGPSFraudAsync(Guid bookingId, double latitude, double longitude);
    
    /// <summary>
    /// Check for booking pattern fraud
    /// </summary>
    Task<bool> CheckBookingFraudAsync(Guid userId, Guid beneficiaryId, Guid caregiverId);
    
    /// <summary>
    /// Check for payment fraud
    /// </summary>
    Task<bool> CheckPaymentFraudAsync(Guid userId, decimal amount);
    
    /// <summary>
    /// Get fraud alerts with optional filtering
    /// </summary>
    Task<List<FraudAlert>> GetAlertsAsync(Guid? userId = null, string? status = null);
    
    /// <summary>
    /// Get fraud score history for a user
    /// </summary>
    Task<List<FraudScore>> GetFraudScoreHistoryAsync(Guid userId);
    
    /// <summary>
    /// Get suspicious activities with optional filtering
    /// </summary>
    Task<List<SuspiciousActivity>> GetSuspiciousActivitiesAsync(Guid? userId = null, int? minRiskScore = null);
    
    /// <summary>
    /// Resolve a fraud alert
    /// </summary>
    Task ResolveAlertAsync(Guid alertId, string resolution, string investigatedBy);
    
    /// <summary>
    /// Create a fraud alert
    /// </summary>
    Task<FraudAlert> CreateAlertAsync(Guid userId, string alertType, int severity, string description);
}
