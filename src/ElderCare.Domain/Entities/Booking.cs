using ElderCare.Domain.Enums;

namespace ElderCare.Domain.Entities;

public class Booking : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid CaregiverId { get; set; }
    public Guid BeneficiaryId { get; set; }
    
    // Schedule
    public DateTime ScheduledStartTime { get; set; }
    public DateTime ScheduledEndTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    
    // Escrow payment tracking
    public decimal? EscrowAmount { get; set; }
    public DateTime? EscrowHeldAt { get; set; }
    public DateTime? EscrowReleasedAt { get; set; }
    public Guid? EscrowTransactionId { get; set; }
    
    // Location
    public string ServiceLocation { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int GeofenceRadiusMeters { get; set; } = 100;
    
    // Check-in/out
    public double? CheckInLatitude { get; set; }
    public double? CheckInLongitude { get; set; }
    public string? CheckInPhotoUrl { get; set; }
    public double? CheckOutLatitude { get; set; }
    public double? CheckOutLongitude { get; set; }
    public string? CheckOutNotes { get; set; }
    
    // Status & Payment
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public decimal TotalAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public string? SpecialRequirements { get; set; }
    
    // AI Matching
    public double? AiMatchScore { get; set; }
    
    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public Caregiver Caregiver { get; set; } = null!;
    public Beneficiary Beneficiary { get; set; } = null!;
    public Review? Review { get; set; }
    public Dispute? Dispute { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<LocationLog> LocationLogs { get; set; } = new List<LocationLog>();
}
