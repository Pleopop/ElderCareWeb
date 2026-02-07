namespace ElderCare.Application.Features.Matching.DTOs;

public class MatchingResultDto
{
    public Guid CaregiverId { get; set; }
    public string CaregiverName { get; set; } = string.Empty;
    public double OverallScore { get; set; }
    public double PersonalityScore { get; set; }
    public double SkillScore { get; set; }
    public double AvailabilityScore { get; set; }
    public double LocationScore { get; set; }
    public double PerformanceScore { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public decimal? HourlyRate { get; set; }
    public int? YearsOfExperience { get; set; }
}

public class CalculateMatchRequest
{
    public Guid BeneficiaryId { get; set; }
}
