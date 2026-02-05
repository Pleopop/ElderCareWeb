using ElderCare.Domain.Enums;

namespace ElderCare.Application.Features.Bookings.DTOs;

// Request DTOs
public class CreateBookingRequest
{
    public Guid BeneficiaryId { get; set; }
    public Guid CaregiverProfileId { get; set; }
    public DateTime ScheduledStartTime { get; set; }
    public DateTime ScheduledEndTime { get; set; }
    public string ServiceLocation { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? SpecialRequirements { get; set; }
}

public class CheckInRequest
{
    public Guid BookingId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? PhotoUrl { get; set; }
}

public class CheckOutRequest
{
    public Guid BookingId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Notes { get; set; }
}

// Response DTOs
public class BookingDto
{
    public Guid Id { get; set; }
    public Guid CustomerProfileId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid CaregiverProfileId { get; set; }
    public string CaregiverName { get; set; } = string.Empty;
    public Guid BeneficiaryId { get; set; }
    public string BeneficiaryName { get; set; } = string.Empty;
    public DateTime ScheduledStartTime { get; set; }
    public DateTime ScheduledEndTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public string ServiceLocation { get; set; } = string.Empty;
    public BookingStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string? SpecialRequirements { get; set; }
    public double? AiMatchScore { get; set; }
    public DateTime CreatedAt { get; set; }
}
