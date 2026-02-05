namespace ElderCare.Domain.Entities;

public class LocationLog : BaseEntity
{
    public Guid BookingId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Accuracy { get; set; }  // GPS accuracy in meters
    public DateTime Timestamp { get; set; }
    
    // Navigation properties
    public Booking Booking { get; set; } = null!;
}
