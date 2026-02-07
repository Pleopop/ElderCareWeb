using ElderCare.Application.Features.CaregiverAssistant.DTOs;

namespace ElderCare.Application.Common.Interfaces;

public interface ICaregiverAssistantService
{
    // Care Notes & Mood Assessment
    Task<CareNoteDto> CreateCareNoteAsync(Guid bookingId, Guid caregiverId, Guid beneficiaryId, string observation, int moodLevel);
    Task<List<CareNoteDto>> GetCareNotesAsync(Guid beneficiaryId, int days = 30);
    Task<MoodTrendDto> GetMoodTrendAsync(Guid beneficiaryId, int days = 30);
    
    // Activity Suggestions
    Task<List<ActivitySuggestionDto>> GenerateActivitySuggestionsAsync(Guid beneficiaryId);
    Task<ActivitySuggestionDto> MarkActivityCompletedAsync(Guid activityId, int engagementRating, string? feedback);
    Task<List<ActivitySuggestionDto>> GetActiveSuggestionsAsync(Guid beneficiaryId);
    
    // Conversation Starters
    Task<List<ConversationStarterDto>> GetConversationStartersAsync(Guid beneficiaryId);
    
    // Daily Reports
    Task<DailyReportDto> GenerateDailyReportAsync(Guid bookingId, DateTime reportDate);
    Task<DailyReportDto> ApproveDailyReportAsync(Guid reportId, string? caregiverNotes);
    Task<List<DailyReportDto>> GetDailyReportsAsync(Guid beneficiaryId, int days = 7);
}
