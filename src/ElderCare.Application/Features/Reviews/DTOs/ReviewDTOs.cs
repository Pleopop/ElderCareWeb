namespace ElderCare.Application.Features.Reviews.DTOs;

public class CreateReviewRequest
{
    public Guid BookingId { get; set; }
    public int OverallRating { get; set; } // 1-5
    public int PunctualityRating { get; set; }
    public int ProfessionalismRating { get; set; }
    public int CommunicationRating { get; set; }
    public int CareQualityRating { get; set; }
    public string? Comment { get; set; }
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid CaregiverId { get; set; }
    public string CaregiverName { get; set; } = string.Empty;
    public int OverallRating { get; set; }
    public int PunctualityRating { get; set; }
    public int ProfessionalismRating { get; set; }
    public int CommunicationRating { get; set; }
    public int CareQualityRating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
