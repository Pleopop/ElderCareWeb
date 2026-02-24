using ElderCare.Domain.Entities;
using ElderCare.Domain.Interfaces;
using System.Text.Json;

namespace ElderCare.Application.Services;

/// <summary>
/// Implementation of fraud detection service using Repository pattern
/// </summary>
public class FraudDetectionService : IFraudDetectionService
{
    private readonly IUnitOfWork _unitOfWork;
    
    // Fraud detection thresholds
    private const double MAX_SPEED_KMH = 100; // Maximum reasonable speed for caregiver
    private const double MAX_DISTANCE_FROM_BENEFICIARY_KM = 5; // Maximum distance from beneficiary location
    private const int MAX_BOOKINGS_PER_DAY = 10; // Maximum bookings per day
    private const decimal MAX_TRANSACTION_AMOUNT = 10_000_000; // 10 million VND
    private const int HIGH_FRAUD_SCORE_THRESHOLD = 70;
    
    public FraudDetectionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<FraudScore> CalculateFraudScoreAsync(Guid userId)
    {
        var gpsScore = await CalculateGPSScoreAsync(userId);
        var bookingScore = await CalculateBookingScoreAsync(userId);
        var paymentScore = await CalculatePaymentScoreAsync(userId);
        var identityScore = await CalculateIdentityScoreAsync(userId);
        
        // Weighted average: GPS 30%, Booking 30%, Payment 30%, Identity 10%
        var overallScore = (gpsScore * 0.3m) + (bookingScore * 0.3m) + 
                          (paymentScore * 0.3m) + (identityScore * 0.1m);
        
        var fraudScore = new FraudScore
        {
            UserId = userId,
            OverallScore = overallScore,
            GPSScore = gpsScore,
            BookingScore = bookingScore,
            PaymentScore = paymentScore,
            IdentityScore = identityScore,
            CalculatedAt = DateTime.UtcNow
        };
        
        await _unitOfWork.Repository<FraudScore>().AddAsync(fraudScore);
        await _unitOfWork.SaveChangesAsync();
        
        // Create alert if score is high
        if (overallScore > HIGH_FRAUD_SCORE_THRESHOLD)
        {
            await CreateAlertAsync(userId, "Overall", 3,
                $"High fraud score detected: {overallScore:F2}/100");
        }
        
        return fraudScore;
    }
    
    public async Task<bool> CheckGPSFraudAsync(Guid bookingId, double latitude, double longitude)
    {
        var booking = await _unitOfWork.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking == null) return false;
        
        var fraudDetected = false;
        
        // Get location logs for this booking
        var locationLogs = await _unitOfWork.Repository<LocationLog>()
            .GetAllAsync(l => l.BookingId == bookingId);
        
        // Check 1: Velocity check - detect impossible travel speeds
        var lastLocation = locationLogs
            .OrderByDescending(l => l.Timestamp)
            .FirstOrDefault();
            
        if (lastLocation != null)
        {
            var distance = CalculateDistance(
                lastLocation.Latitude, lastLocation.Longitude,
                latitude, longitude
            );
            
            var timeDiff = (DateTime.UtcNow - lastLocation.Timestamp).TotalHours;
            
            if (timeDiff > 0)
            {
                var speed = distance / timeDiff; // km/h
                
                // Flag if speed > 100 km/h (impossible for caregiver)
                if (speed > MAX_SPEED_KMH)
                {
                    await CreateAlertAsync(booking.CaregiverId, "GPS", 3,
                        $"Impossible travel speed: {speed:F2} km/h between locations");
                    
                    await LogSuspiciousActivityAsync(booking.CaregiverId, "VelocityAnomaly", new
                    {
                        BookingId = bookingId,
                        Speed = speed,
                        Distance = distance,
                        TimeDiff = timeDiff,
                        PreviousLocation = new { lastLocation.Latitude, lastLocation.Longitude },
                        CurrentLocation = new { Latitude = latitude, Longitude = longitude }
                    }, 80);
                    
                    fraudDetected = true;
                }
            }
        }
        
        return fraudDetected;
    }
    
    public async Task<bool> CheckBookingFraudAsync(Guid userId, Guid beneficiaryId, Guid caregiverId)
    {
        var fraudDetected = false;
        
        // Check 1: Rapid booking pattern - multiple bookings in short time
        var recentBookings = (await _unitOfWork.Bookings
            .GetAllAsync(b => b.CustomerId == userId && 
                             b.CreatedAt >= DateTime.UtcNow.AddDays(-1)))
            .Count();
            
        if (recentBookings > MAX_BOOKINGS_PER_DAY)
        {
            await CreateAlertAsync(userId, "Booking", 2,
                $"Unusual booking pattern: {recentBookings} bookings in 24 hours");
            
            await LogSuspiciousActivityAsync(userId, "RapidBooking", new
            {
                BookingCount = recentBookings,
                TimeWindow = "24 hours"
            }, 50);
            
            fraudDetected = true;
        }
        
        // Check 2: Cancellation rate - high cancellation frequency
        var allBookings = await _unitOfWork.Bookings.GetByCustomerIdAsync(userId);
        var totalBookings = allBookings.Count();
        var cancelledBookings = allBookings.Count(b => b.Status == Domain.Enums.BookingStatus.Cancelled);
            
        if (totalBookings > 5)
        {
            var cancellationRate = (decimal)cancelledBookings / totalBookings;
            
            if (cancellationRate > 0.5m) // More than 50% cancellation rate
            {
                await CreateAlertAsync(userId, "Booking", 2,
                    $"High cancellation rate: {cancellationRate:P0} ({cancelledBookings}/{totalBookings})");
                
                await LogSuspiciousActivityAsync(userId, "HighCancellationRate", new
                {
                    CancellationRate = cancellationRate,
                    TotalBookings = totalBookings,
                    CancelledBookings = cancelledBookings
                }, 55);
                
                fraudDetected = true;
            }
        }
        
        return fraudDetected;
    }
    
    public async Task<bool> CheckPaymentFraudAsync(Guid userId, decimal amount)
    {
        var fraudDetected = false;
        
        // Check 1: Amount anomaly - unusual transaction amounts
        if (amount > MAX_TRANSACTION_AMOUNT)
        {
            await CreateAlertAsync(userId, "Payment", 3,
                $"Unusually large transaction: {amount:N0} VND");
            
            await LogSuspiciousActivityAsync(userId, "LargeTransaction", new
            {
                Amount = amount,
                Threshold = MAX_TRANSACTION_AMOUNT
            }, 70);
            
            fraudDetected = true;
        }
        
        // Check 2: Transaction velocity - multiple transactions in short time
        var recentTransactions = (await _unitOfWork.Transactions
            .GetAllAsync(t => t.WalletId == userId && 
                             t.CreatedAt >= DateTime.UtcNow.AddHours(-1)))
            .Count();
            
        if (recentTransactions > 10)
        {
            await CreateAlertAsync(userId, "Payment", 2,
                $"High transaction velocity: {recentTransactions} transactions in 1 hour");
            
            await LogSuspiciousActivityAsync(userId, "TransactionVelocity", new
            {
                TransactionCount = recentTransactions,
                TimeWindow = "1 hour"
            }, 60);
            
            fraudDetected = true;
        }
        
        return fraudDetected;
    }
    
    public async Task<List<FraudAlert>> GetAlertsAsync(Guid? userId = null, string? status = null)
    {
        var alerts = await _unitOfWork.Repository<FraudAlert>().GetAllAsync();
        
        var query = alerts.AsQueryable();
            
        if (userId.HasValue)
        {
            query = query.Where(f => f.UserId == userId.Value);
        }
        
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(f => f.Status == status);
        }
        
        return query
            .OrderByDescending(f => f.DetectedAt)
            .ToList();
    }
    
    public async Task<List<FraudScore>> GetFraudScoreHistoryAsync(Guid userId)
    {
        var scores = await _unitOfWork.Repository<FraudScore>()
            .GetAllAsync(f => f.UserId == userId);
            
        return scores
            .OrderByDescending(f => f.CalculatedAt)
            .Take(10) // Last 10 scores
            .ToList();
    }
    
    public async Task<List<SuspiciousActivity>> GetSuspiciousActivitiesAsync(Guid? userId = null, int? minRiskScore = null)
    {
        var activities = await _unitOfWork.Repository<SuspiciousActivity>().GetAllAsync();
        
        var query = activities.AsQueryable();
        
        if (userId.HasValue)
        {
            query = query.Where(s => s.UserId == userId.Value);
        }
        
        if (minRiskScore.HasValue)
        {
            query = query.Where(s => s.RiskScore >= minRiskScore.Value);
        }
        
        return query
            .OrderByDescending(s => s.DetectedAt)
            .Take(50) // Last 50 activities
            .ToList();
    }
    
    public async Task ResolveAlertAsync(Guid alertId, string resolution, string investigatedBy)
    {
        var alert = await _unitOfWork.Repository<FraudAlert>().GetByIdAsync(alertId);
        
        if (alert == null)
        {
            throw new InvalidOperationException("Fraud alert not found");
        }
        
        alert.Status = "Resolved";
        alert.Resolution = resolution;
        alert.InvestigatedBy = investigatedBy;
        alert.InvestigatedAt = DateTime.UtcNow;
        alert.UpdatedAt = DateTime.UtcNow;
        
        await _unitOfWork.Repository<FraudAlert>().UpdateAsync(alert);
        await _unitOfWork.SaveChangesAsync();
    }
    
    public async Task<FraudAlert> CreateAlertAsync(Guid userId, string alertType, int severity, string description)
    {
        var alert = new FraudAlert
        {
            UserId = userId,
            AlertType = alertType,
            Severity = severity,
            Description = description,
            DetectedAt = DateTime.UtcNow,
            Status = "Pending"
        };
        
        await _unitOfWork.Repository<FraudAlert>().AddAsync(alert);
        await _unitOfWork.SaveChangesAsync();
        
        return alert;
    }
    
    // Private helper methods
    
    private async Task<decimal> CalculateGPSScoreAsync(Guid userId)
    {
        // Check for GPS-related alerts in the last 30 days
        var gpsAlerts = (await _unitOfWork.Repository<FraudAlert>()
            .GetAllAsync(f => f.UserId == userId && 
                             f.AlertType == "GPS" &&
                             f.DetectedAt >= DateTime.UtcNow.AddDays(-30)))
            .Count();
            
        // Score increases with number of alerts (max 100)
        return Math.Min(gpsAlerts * 20m, 100m);
    }
    
    private async Task<decimal> CalculateBookingScoreAsync(Guid userId)
    {
        var bookingAlerts = (await _unitOfWork.Repository<FraudAlert>()
            .GetAllAsync(f => f.UserId == userId && 
                             f.AlertType == "Booking" &&
                             f.DetectedAt >= DateTime.UtcNow.AddDays(-30)))
            .Count();
            
        return Math.Min(bookingAlerts * 15m, 100m);
    }
    
    private async Task<decimal> CalculatePaymentScoreAsync(Guid userId)
    {
        var paymentAlerts = (await _unitOfWork.Repository<FraudAlert>()
            .GetAllAsync(f => f.UserId == userId && 
                             f.AlertType == "Payment" &&
                             f.DetectedAt >= DateTime.UtcNow.AddDays(-30)))
            .Count();
            
        return Math.Min(paymentAlerts * 25m, 100m);
    }
    
    private async Task<decimal> CalculateIdentityScoreAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        
        if (user == null) return 0;
        
        decimal score = 0;
        
        // Check if email is verified
        if (!user.IsEmailVerified)
        {
            score += 30;
        }
        
        // Check account age (new accounts are riskier)
        var accountAge = (DateTime.UtcNow - user.CreatedAt).TotalDays;
        if (accountAge < 7)
        {
            score += 40;
        }
        else if (accountAge < 30)
        {
            score += 20;
        }
        
        return Math.Min(score, 100m);
    }
    
    private async Task LogSuspiciousActivityAsync(Guid userId, string activityType, object details, decimal riskScore)
    {
        var activity = new SuspiciousActivity
        {
            UserId = userId,
            ActivityType = activityType,
            Details = JsonSerializer.Serialize(details),
            RiskScore = riskScore,
            DetectedAt = DateTime.UtcNow
        };
        
        await _unitOfWork.Repository<SuspiciousActivity>().AddAsync(activity);
        await _unitOfWork.SaveChangesAsync();
    }
    
    /// <summary>
    /// Calculate distance between two GPS coordinates using Haversine formula
    /// </summary>
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in kilometers
        
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return R * c; // Distance in kilometers
    }
    
    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
