using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Features.CaregiverAssistant.DTOs;
using ElderCare.Domain.Entities;
using ElderCare.Domain.Enums;
using ElderCare.Domain.Interfaces;
using Microsoft.ML;
using System.Text.Json;

namespace ElderCare.Application.Services;

/// <summary>
/// ML.NET-based AI Caregiver Assistant Service
/// Provides mood analysis, activity recommendations, and conversation starters
/// Note: This is a simplified version. For production, train models with real data.
/// </summary>
public class MLNetCaregiverAssistantService : ICaregiverAssistantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MLContext _mlContext;

    public MLNetCaregiverAssistantService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mlContext = new MLContext(seed: 0);
    }

    #region Care Notes & Mood Assessment

    public async Task<CareNoteDto> CreateCareNoteAsync(Guid bookingId, Guid caregiverId, Guid beneficiaryId, string observation, int moodLevel)
    {
        // Simple sentiment analysis using keyword matching (ML.NET would require trained model)
        var sentimentScore = AnalyzeSentiment(observation);
        var emotions = DetectEmotions(observation);
        var suggestedActions = GenerateSuggestedActions(moodLevel, emotions);
        
        var careNote = new CareNote
        {
            BookingId = bookingId,
            CaregiverId = caregiverId,
            BeneficiaryId = beneficiaryId,
            Observation = observation,
            AssessedMood = (MoodLevel)moodLevel,
            ObservedAt = DateTime.UtcNow,
            AiMoodAnalysis = GenerateMoodAnalysis(moodLevel, observation),
            SentimentScore = sentimentScore,
            DetectedEmotions = emotions,
            SuggestedActions = suggestedActions,
            RequiresAttention = moodLevel <= 2 || sentimentScore < -0.5
        };

        await _unitOfWork.Repository<CareNote>().AddAsync(careNote);
        await _unitOfWork.SaveChangesAsync();

        return MapToCareNoteDto(careNote);
    }

    public async Task<List<CareNoteDto>> GetCareNotesAsync(Guid beneficiaryId, int days = 30)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        var notes = await _unitOfWork.Repository<CareNote>()
            .GetAllAsync(c => c.BeneficiaryId == beneficiaryId && c.ObservedAt >= startDate);

        return notes.Select(MapToCareNoteDto).OrderByDescending(n => n.ObservedAt).ToList();
    }

    public async Task<MoodTrendDto> GetMoodTrendAsync(Guid beneficiaryId, int days = 30)
    {
        var notes = await GetCareNotesAsync(beneficiaryId, days);
        var beneficiary = await _unitOfWork.Beneficiaries.GetByIdAsync(beneficiaryId);

        if (!notes.Any())
        {
            return new MoodTrendDto
            {
                BeneficiaryId = beneficiaryId,
                BeneficiaryName = beneficiary?.FullName ?? "Unknown",
                MoodHistory = new List<MoodDataPoint>(),
                AverageMood = 3.0,
                Trend = "No Data",
                Insights = new List<string> { "Not enough data to analyze mood trends." }
            };
        }

        var moodHistory = notes.Select(n => new MoodDataPoint
        {
            Date = n.ObservedAt,
            MoodLevel = ParseMoodLevel(n.AssessedMood),
            Note = n.Observation.Length > 50 ? n.Observation.Substring(0, 50) + "..." : n.Observation
        }).ToList();

        var avgMood = moodHistory.Average(m => m.MoodLevel);
        var trend = CalculateTrend(moodHistory);
        var insights = GenerateMoodInsights(moodHistory, trend);

        return new MoodTrendDto
        {
            BeneficiaryId = beneficiaryId,
            BeneficiaryName = beneficiary?.FullName ?? "Unknown",
            MoodHistory = moodHistory,
            AverageMood = Math.Round(avgMood, 2),
            Trend = trend,
            Insights = insights
        };
    }

    #endregion

    #region Activity Suggestions

    public async Task<List<ActivitySuggestionDto>> GenerateActivitySuggestionsAsync(Guid beneficiaryId)
    {
        var beneficiary = await _unitOfWork.Beneficiaries.GetByIdAsync(beneficiaryId);
        if (beneficiary == null) return new List<ActivitySuggestionDto>();

        // Get recent mood to personalize suggestions
        var recentNotes = await GetCareNotesAsync(beneficiaryId, 7);
        var avgMood = recentNotes.Any() ? recentNotes.Average(n => ParseMoodLevel(n.AssessedMood)) : 3.0;

        // Generate suggestions based on personality traits and hobbies
        var suggestions = GeneratePersonalizedActivities(beneficiary, avgMood);

        // Save to database
        foreach (var suggestion in suggestions)
        {
            var entity = new ActivitySuggestion
            {
                BeneficiaryId = beneficiaryId,
                Title = suggestion.Title,
                Description = suggestion.Description,
                Category = Enum.Parse<ActivityCategory>(suggestion.Category),
                DurationMinutes = suggestion.DurationMinutes,
                Difficulty = Enum.Parse<DifficultyLevel>(suggestion.Difficulty),
                AiReasoning = suggestion.AiReasoning,
                ConfidenceScore = suggestion.ConfidenceScore,
                BasedOnTraits = suggestion.BasedOnTraits,
                BasedOnHobbies = suggestion.BasedOnHobbies,
                GeneratedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _unitOfWork.Repository<ActivitySuggestion>().AddAsync(entity);
        }

        await _unitOfWork.SaveChangesAsync();
        return suggestions;
    }

    public async Task<ActivitySuggestionDto> MarkActivityCompletedAsync(Guid activityId, int engagementRating, string? feedback)
    {
        var activity = await _unitOfWork.Repository<ActivitySuggestion>().GetByIdAsync(activityId);
        if (activity == null) throw new Exception("Activity not found");

        activity.IsCompleted = true;
        activity.CompletedAt = DateTime.UtcNow;
        activity.BeneficiaryEngagementRating = engagementRating;
        activity.CaregiverFeedback = feedback;

        await _unitOfWork.Repository<ActivitySuggestion>().UpdateAsync(activity);
        await _unitOfWork.SaveChangesAsync();

        return MapToActivitySuggestionDto(activity);
    }

    public async Task<List<ActivitySuggestionDto>> GetActiveSuggestionsAsync(Guid beneficiaryId)
    {
        var now = DateTime.UtcNow;
        var suggestions = await _unitOfWork.Repository<ActivitySuggestion>()
            .GetAllAsync(a => a.BeneficiaryId == beneficiaryId && 
                             !a.IsCompleted && 
                             (a.ExpiresAt == null || a.ExpiresAt > now));

        return suggestions.Select(MapToActivitySuggestionDto).ToList();
    }

    #endregion

    #region Conversation Starters

    public async Task<List<ConversationStarterDto>> GetConversationStartersAsync(Guid beneficiaryId)
    {
        var beneficiary = await _unitOfWork.Beneficiaries.GetByIdAsync(beneficiaryId);
        if (beneficiary == null) return new List<ConversationStarterDto>();

        // Generate conversation starters based on hobbies and personality
        var starters = new List<ConversationStarterDto>();

        if (!string.IsNullOrEmpty(beneficiary.Hobbies))
        {
            starters.Add(new ConversationStarterDto
            {
                Topic = "Hobbies & Interests",
                Questions = new List<string>
                {
                    $"I noticed you enjoy {beneficiary.Hobbies}. Can you tell me more about that?",
                    "What's your favorite memory related to this hobby?",
                    "Would you like to do this activity together today?"
                },
                Reasoning = "Discussing hobbies helps build rapport and engagement"
            });
        }

        starters.Add(new ConversationStarterDto
        {
            Topic = "Family & Memories",
            Questions = new List<string>
            {
                "Tell me about your family. Do you have any children or grandchildren?",
                "What's your favorite family memory?",
                "What was life like when you were younger?"
            },
            Reasoning = "Reminiscing about family creates positive emotions"
        });

        starters.Add(new ConversationStarterDto
        {
            Topic = "Daily Routine",
            Questions = new List<string>
            {
                "How did you sleep last night?",
                "What would you like to do today?",
                "Is there anything special you'd like for lunch?"
            },
            Reasoning = "Discussing daily activities shows care and involvement"
        });

        return starters;
    }

    #endregion

    #region Daily Reports

    public async Task<DailyReportDto> GenerateDailyReportAsync(Guid bookingId, DateTime reportDate)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null) throw new Exception("Booking not found");

        // Get care notes for the day
        var dayStart = reportDate.Date;
        var dayEnd = dayStart.AddDays(1);
        var careNotes = await _unitOfWork.Repository<CareNote>()
            .GetAllAsync(c => c.BookingId == bookingId && 
                             c.ObservedAt >= dayStart && 
                             c.ObservedAt < dayEnd);

        // Get completed activities
        var activities = await _unitOfWork.Repository<ActivitySuggestion>()
            .GetAllAsync(a => a.BeneficiaryId == booking.BeneficiaryId && 
                             a.IsCompleted && 
                             a.CompletedAt >= dayStart && 
                             a.CompletedAt < dayEnd);

        // Generate AI summary
        var summary = GenerateDailySummary(careNotes.ToList(), activities.ToList());
        var avgMood = careNotes.Any() ? (MoodLevel)(int)Math.Round(careNotes.Average(c => (int)c.AssessedMood)) : MoodLevel.Neutral;
        var insights = GenerateDailyInsights(careNotes.ToList());

        var report = new DailyReport
        {
            BookingId = bookingId,
            CaregiverId = booking.CaregiverId,
            BeneficiaryId = booking.BeneficiaryId,
            CustomerId = booking.CustomerId,
            ReportDate = reportDate.Date,
            Summary = summary,
            AverageMood = avgMood,
            ActivitiesCompleted = activities.Select(a => a.Title).ToList(),
            AiInsights = string.Join("; ", insights),
            PositiveHighlights = insights.Where(i => i.Contains("positive") || i.Contains("good")).ToList(),
            AreasOfConcern = insights.Where(i => i.Contains("concern") || i.Contains("low")).ToList(),
            GeneratedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<DailyReport>().AddAsync(report);
        await _unitOfWork.SaveChangesAsync();

        return MapToDailyReportDto(report);
    }

    public async Task<DailyReportDto> ApproveDailyReportAsync(Guid reportId, string? caregiverNotes)
    {
        var report = await _unitOfWork.Repository<DailyReport>().GetByIdAsync(reportId);
        if (report == null) throw new Exception("Report not found");

        report.CaregiverApproved = true;
        report.ApprovedAt = DateTime.UtcNow;
        report.CaregiverNotes = caregiverNotes;

        await _unitOfWork.Repository<DailyReport>().UpdateAsync(report);
        await _unitOfWork.SaveChangesAsync();

        return MapToDailyReportDto(report);
    }

    public async Task<List<DailyReportDto>> GetDailyReportsAsync(Guid beneficiaryId, int days = 7)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days);
        var reports = await _unitOfWork.Repository<DailyReport>()
            .GetAllAsync(r => r.BeneficiaryId == beneficiaryId && r.ReportDate >= startDate);

        return reports.Select(MapToDailyReportDto).OrderByDescending(r => r.ReportDate).ToList();
    }

    #endregion

    #region Helper Methods - Simplified ML Logic

    private double AnalyzeSentiment(string text)
    {
        // Simplified sentiment analysis (in production, use trained ML model)
        var positiveWords = new[] { "happy", "good", "great", "wonderful", "cheerful", "smiling", "active", "engaged" };
        var negativeWords = new[] { "sad", "upset", "angry", "tired", "withdrawn", "refused", "crying", "agitated" };

        var lowerText = text.ToLower();
        var positiveCount = positiveWords.Count(w => lowerText.Contains(w));
        var negativeCount = negativeWords.Count(w => lowerText.Contains(w));

        if (positiveCount + negativeCount == 0) return 0.0;
        return (double)(positiveCount - negativeCount) / (positiveCount + negativeCount);
    }

    private List<string> DetectEmotions(string text)
    {
        var emotions = new List<string>();
        var lowerText = text.ToLower();

        if (lowerText.Contains("happy") || lowerText.Contains("smiling") || lowerText.Contains("cheerful"))
            emotions.Add("happy");
        if (lowerText.Contains("sad") || lowerText.Contains("crying") || lowerText.Contains("upset"))
            emotions.Add("sad");
        if (lowerText.Contains("calm") || lowerText.Contains("peaceful") || lowerText.Contains("relaxed"))
            emotions.Add("calm");
        if (lowerText.Contains("anxious") || lowerText.Contains("worried") || lowerText.Contains("nervous"))
            emotions.Add("anxious");
        if (lowerText.Contains("engaged") || lowerText.Contains("active") || lowerText.Contains("interested"))
            emotions.Add("engaged");

        return emotions.Any() ? emotions : new List<string> { "neutral" };
    }

    private List<string> GenerateSuggestedActions(int moodLevel, List<string> emotions)
    {
        var actions = new List<string>();

        if (moodLevel <= 2)
        {
            actions.Add("Spend extra time with beneficiary");
            actions.Add("Engage in favorite activities");
            actions.Add("Consider notifying family if mood persists");
        }

        if (emotions.Contains("anxious"))
        {
            actions.Add("Provide reassurance and calm environment");
            actions.Add("Try relaxation activities (music, gentle exercise)");
        }

        if (emotions.Contains("engaged"))
        {
            actions.Add("Continue current activities");
            actions.Add("Introduce similar engaging activities");
        }

        return actions.Any() ? actions : new List<string> { "Continue regular care routine" };
    }

    private string GenerateMoodAnalysis(int moodLevel, string observation)
    {
        return moodLevel switch
        {
            1 => "Very low mood detected. Beneficiary may need extra attention and support.",
            2 => "Low mood observed. Consider engaging activities to lift spirits.",
            3 => "Neutral mood. Beneficiary appears stable.",
            4 => "Good mood. Beneficiary is doing well today.",
            5 => "Excellent mood! Beneficiary is very happy and engaged.",
            _ => "Mood assessment unavailable."
        };
    }

    private List<ActivitySuggestionDto> GeneratePersonalizedActivities(Beneficiary beneficiary, double avgMood)
    {
        var suggestions = new List<ActivitySuggestionDto>();
        var hobbies = beneficiary.Hobbies?.Split(',').Select(h => h.Trim().ToLower()).ToList() ?? new List<string>();
        var traits = beneficiary.PersonalityTraits?.Split(',').Select(t => t.Trim().ToLower()).ToList() ?? new List<string>();

        // Music activity
        if (hobbies.Contains("music") || traits.Contains("creative"))
        {
            suggestions.Add(new ActivitySuggestionDto
            {
                Title = "Listen to Favorite Music",
                Description = "Play some of their favorite songs from their youth. Music can improve mood and trigger positive memories.",
                Category = "Entertainment",
                DurationMinutes = 30,
                Difficulty = "VeryEasy",
                AiReasoning = "Based on interest in music and creative personality",
                ConfidenceScore = 0.85,
                BasedOnTraits = new List<string> { "creative" },
                BasedOnHobbies = new List<string> { "music" },
                GeneratedAt = DateTime.UtcNow
            });
        }

        // Physical activity (if mood is good)
        if (avgMood >= 3)
        {
            suggestions.Add(new ActivitySuggestionDto
            {
                Title = "Gentle Walking",
                Description = "Take a short walk around the house or garden. Fresh air and light exercise can boost mood.",
                Category = "Physical",
                DurationMinutes = 15,
                Difficulty = "Easy",
                AiReasoning = "Mood is stable, suitable for light physical activity",
                ConfidenceScore = 0.75,
                BasedOnTraits = new List<string>(),
                BasedOnHobbies = new List<string>(),
                GeneratedAt = DateTime.UtcNow
            });
        }

        // Social activity
        suggestions.Add(new ActivitySuggestionDto
        {
            Title = "Conversation Time",
            Description = "Have a meaningful conversation about their life, family, or favorite memories.",
            Category = "Social",
            DurationMinutes = 20,
            Difficulty = "VeryEasy",
            AiReasoning = "Social interaction is beneficial for all elderly individuals",
            ConfidenceScore = 0.90,
            BasedOnTraits = new List<string>(),
            BasedOnHobbies = new List<string>(),
            GeneratedAt = DateTime.UtcNow
        });

        return suggestions;
    }

    private string CalculateTrend(List<MoodDataPoint> moodHistory)
    {
        if (moodHistory.Count < 3) return "Insufficient Data";

        var recent = moodHistory.Take(7).Average(m => m.MoodLevel);
        var older = moodHistory.Skip(7).Take(7).Average(m => m.MoodLevel);

        if (recent > older + 0.5) return "Improving";
        if (recent < older - 0.5) return "Declining";
        return "Stable";
    }

    private List<string> GenerateMoodInsights(List<MoodDataPoint> moodHistory, string trend)
    {
        var insights = new List<string>();
        var avgMood = moodHistory.Average(m => m.MoodLevel);

        insights.Add($"Average mood over the period: {avgMood:F1}/5");
        insights.Add($"Mood trend: {trend}");

        if (trend == "Declining")
            insights.Add("Consider increasing engagement activities and monitoring closely");
        else if (trend == "Improving")
            insights.Add("Current care approach is working well, continue current activities");

        return insights;
    }

    private string GenerateDailySummary(List<CareNote> careNotes, List<ActivitySuggestion> activities)
    {
        if (!careNotes.Any())
            return "No observations recorded for this day.";

        var avgMood = (int)Math.Round(careNotes.Average(c => (int)c.AssessedMood));
        var moodText = avgMood switch
        {
            1 or 2 => "had a challenging day",
            3 => "had a stable day",
            4 or 5 => "had a good day",
            _ => "was observed"
        };

        var activityText = activities.Any() 
            ? $" Completed {activities.Count} activities including {string.Join(", ", activities.Take(3).Select(a => a.Title))}."
            : " No structured activities completed.";

        return $"Beneficiary {moodText}.{activityText} Overall mood: {avgMood}/5.";
    }

    private List<string> GenerateDailyInsights(List<CareNote> careNotes)
    {
        var insights = new List<string>();

        if (!careNotes.Any())
        {
            insights.Add("No data available for analysis");
            return insights;
        }

        var avgMood = careNotes.Average(c => (int)c.AssessedMood);
        if (avgMood >= 4)
            insights.Add("Positive mood throughout the day");
        else if (avgMood <= 2)
            insights.Add("Low mood observed - may need additional support");

        var requiresAttention = careNotes.Any(c => c.RequiresAttention);
        if (requiresAttention)
            insights.Add("Some observations flagged for attention");

        return insights;
    }

    private int ParseMoodLevel(string moodLevel)
    {
        return Enum.TryParse<MoodLevel>(moodLevel, out var mood) ? (int)mood : 3;
    }

    #endregion

    #region Mapping Methods

    private CareNoteDto MapToCareNoteDto(CareNote careNote)
    {
        return new CareNoteDto
        {
            Id = careNote.Id,
            BookingId = careNote.BookingId,
            CaregiverId = careNote.CaregiverId,
            BeneficiaryId = careNote.BeneficiaryId,
            Observation = careNote.Observation,
            AssessedMood = careNote.AssessedMood.ToString(),
            ObservedAt = careNote.ObservedAt,
            AiMoodAnalysis = careNote.AiMoodAnalysis,
            SentimentScore = careNote.SentimentScore,
            DetectedEmotions = careNote.DetectedEmotions,
            SuggestedActions = careNote.SuggestedActions,
            RequiresAttention = careNote.RequiresAttention
        };
    }

    private ActivitySuggestionDto MapToActivitySuggestionDto(ActivitySuggestion activity)
    {
        return new ActivitySuggestionDto
        {
            Id = activity.Id,
            BeneficiaryId = activity.BeneficiaryId,
            Title = activity.Title,
            Description = activity.Description,
            Category = activity.Category.ToString(),
            DurationMinutes = activity.DurationMinutes,
            Difficulty = activity.Difficulty.ToString(),
            AiReasoning = activity.AiReasoning,
            ConfidenceScore = activity.ConfidenceScore,
            BasedOnTraits = activity.BasedOnTraits,
            BasedOnHobbies = activity.BasedOnHobbies,
            IsCompleted = activity.IsCompleted,
            GeneratedAt = activity.GeneratedAt
        };
    }

    private DailyReportDto MapToDailyReportDto(DailyReport report)
    {
        return new DailyReportDto
        {
            Id = report.Id,
            BookingId = report.BookingId,
            BeneficiaryId = report.BeneficiaryId,
            BeneficiaryName = "", // Will be populated by controller
            ReportDate = report.ReportDate,
            Summary = report.Summary,
            AverageMood = report.AverageMood.ToString(),
            ActivitiesCompleted = report.ActivitiesCompleted,
            MealsConsumed = report.MealsConsumed,
            HealthNotes = report.HealthNotes,
            BehaviorNotes = report.BehaviorNotes,
            AiInsights = report.AiInsights,
            PositiveHighlights = report.PositiveHighlights,
            AreasOfConcern = report.AreasOfConcern,
            CaregiverNotes = report.CaregiverNotes,
            CaregiverApproved = report.CaregiverApproved,
            ViewedByCustomer = report.ViewedByCustomer
        };
    }

    #endregion
}
