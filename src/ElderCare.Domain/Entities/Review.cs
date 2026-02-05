namespace ElderCare.Domain.Entities;

public class Review : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid CaregiverId { get; set; }
    public int OverallRating { get; set; } // 1-5
    public int PunctualityRating { get; set; } // 1-5
    public int ProfessionalismRating { get; set; } // 1-5
    public int CommunicationRating { get; set; } // 1-5
    public int CareQualityRating { get; set; } // 1-5
    public string? Comment { get; set; }
    
    public Booking Booking { get; set; } = null!;
}
