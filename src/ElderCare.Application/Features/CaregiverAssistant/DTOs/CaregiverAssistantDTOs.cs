namespace ElderCare.Application.Features.CaregiverAssistant.DTOs;

public class CareNoteDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid CaregiverId { get; set; }
    public Guid BeneficiaryId { get; set; }
    public string Observation { get; set; } = string.Empty;
    public string AssessedMood { get; set; } = string.Empty;
    public DateTime ObservedAt { get; set; }
    public string? AiMoodAnalysis { get; set; }
    public double? SentimentScore { get; set; }
    public List<string>? DetectedEmotions { get; set; }
    public List<string>? SuggestedActions { get; set; }
    public bool RequiresAttention { get; set; }
}

public class ActivitySuggestionDto
{
    public Guid Id { get; set; }
    public Guid BeneficiaryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Difficulty { get; set; } = string.Empty;
    public string? AiReasoning { get; set; }
    public double ConfidenceScore { get; set; }
    public List<string>? BasedOnTraits { get; set; }
    public List<string>? BasedOnHobbies { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class DailyReportDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid BeneficiaryId { get; set; }
    public string BeneficiaryName { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string AverageMood { get; set; } = string.Empty;
    public List<string>? ActivitiesCompleted { get; set; }
    public List<string>? MealsConsumed { get; set; }
    public string? HealthNotes { get; set; }
    public string? BehaviorNotes { get; set; }
    public string? AiInsights { get; set; }
    public List<string>? PositiveHighlights { get; set; }
    public List<string>? AreasOfConcern { get; set; }
    public string? CaregiverNotes { get; set; }
    public bool CaregiverApproved { get; set; }
    public bool ViewedByCustomer { get; set; }
}

public class MoodTrendDto
{
    public Guid BeneficiaryId { get; set; }
    public string BeneficiaryName { get; set; } = string.Empty;
    public List<MoodDataPoint> MoodHistory { get; set; } = new();
    public double AverageMood { get; set; }
    public string Trend { get; set; } = string.Empty; // "Improving", "Stable", "Declining"
    public List<string>? Insights { get; set; }
}

public class MoodDataPoint
{
    public DateTime Date { get; set; }
    public int MoodLevel { get; set; }
    public string? Note { get; set; }
}

public class ConversationStarterDto
{
    public string Topic { get; set; } = string.Empty;
    public List<string> Questions { get; set; } = new();
    public string Reasoning { get; set; } = string.Empty;
}
