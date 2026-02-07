using ElderCare.Application.Common.Interfaces;
using ElderCare.Application.Features.CaregiverAssistant.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElderCare.API.Controllers;

[ApiController]
[Route("api/caregiver-assistant")]
[Authorize]
public class CaregiverAssistantController : ControllerBase
{
    private readonly ICaregiverAssistantService _assistantService;

    public CaregiverAssistantController(ICaregiverAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    #region Care Notes & Mood Assessment

    /// <summary>
    /// Create a care note with AI mood analysis
    /// </summary>
    [HttpPost("care-notes")]
    [Authorize(Roles = "Caregiver")]
    public async Task<ActionResult<CareNoteDto>> CreateCareNote([FromBody] CreateCareNoteRequest request)
    {
        var caregiverIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(caregiverIdClaim) || !Guid.TryParse(caregiverIdClaim, out var caregiverId))
            return Unauthorized();

        var result = await _assistantService.CreateCareNoteAsync(
            request.BookingId,
            caregiverId,
            request.BeneficiaryId,
            request.Observation,
            request.MoodLevel
        );

        return Ok(result);
    }

    /// <summary>
    /// Get care notes for a beneficiary
    /// </summary>
    [HttpGet("care-notes/{beneficiaryId}")]
    [Authorize(Roles = "Caregiver,Customer")]
    public async Task<ActionResult<List<CareNoteDto>>> GetCareNotes(Guid beneficiaryId, [FromQuery] int days = 30)
    {
        var result = await _assistantService.GetCareNotesAsync(beneficiaryId, days);
        return Ok(result);
    }

    /// <summary>
    /// Get mood trend analysis for a beneficiary
    /// </summary>
    [HttpGet("mood-trend/{beneficiaryId}")]
    [Authorize(Roles = "Caregiver,Customer")]
    public async Task<ActionResult<MoodTrendDto>> GetMoodTrend(Guid beneficiaryId, [FromQuery] int days = 30)
    {
        var result = await _assistantService.GetMoodTrendAsync(beneficiaryId, days);
        return Ok(result);
    }

    #endregion

    #region Activity Suggestions

    /// <summary>
    /// Generate AI-powered activity suggestions for a beneficiary
    /// </summary>
    [HttpPost("activity-suggestions/{beneficiaryId}")]
    [Authorize(Roles = "Caregiver")]
    public async Task<ActionResult<List<ActivitySuggestionDto>>> GenerateActivitySuggestions(Guid beneficiaryId)
    {
        var result = await _assistantService.GenerateActivitySuggestionsAsync(beneficiaryId);
        return Ok(result);
    }

    /// <summary>
    /// Get active activity suggestions for a beneficiary
    /// </summary>
    [HttpGet("activity-suggestions/{beneficiaryId}")]
    [Authorize(Roles = "Caregiver")]
    public async Task<ActionResult<List<ActivitySuggestionDto>>> GetActiveSuggestions(Guid beneficiaryId)
    {
        var result = await _assistantService.GetActiveSuggestionsAsync(beneficiaryId);
        return Ok(result);
    }

    /// <summary>
    /// Mark an activity as completed with engagement rating
    /// </summary>
    [HttpPut("activity-suggestions/{activityId}/complete")]
    [Authorize(Roles = "Caregiver")]
    public async Task<ActionResult<ActivitySuggestionDto>> MarkActivityCompleted(
        Guid activityId,
        [FromBody] CompleteActivityRequest request)
    {
        var result = await _assistantService.MarkActivityCompletedAsync(
            activityId,
            request.EngagementRating,
            request.Feedback
        );
        return Ok(result);
    }

    #endregion

    #region Conversation Starters

    /// <summary>
    /// Get AI-generated conversation starters for a beneficiary
    /// </summary>
    [HttpGet("conversation-starters/{beneficiaryId}")]
    [Authorize(Roles = "Caregiver")]
    public async Task<ActionResult<List<ConversationStarterDto>>> GetConversationStarters(Guid beneficiaryId)
    {
        var result = await _assistantService.GetConversationStartersAsync(beneficiaryId);
        return Ok(result);
    }

    #endregion

    #region Daily Reports

    /// <summary>
    /// Generate AI-assisted daily care report
    /// </summary>
    [HttpPost("daily-reports")]
    [Authorize(Roles = "Caregiver")]
    public async Task<ActionResult<DailyReportDto>> GenerateDailyReport([FromBody] GenerateReportRequest request)
    {
        var result = await _assistantService.GenerateDailyReportAsync(request.BookingId, request.ReportDate);
        return Ok(result);
    }

    /// <summary>
    /// Approve and finalize a daily report
    /// </summary>
    [HttpPut("daily-reports/{reportId}/approve")]
    [Authorize(Roles = "Caregiver")]
    public async Task<ActionResult<DailyReportDto>> ApproveDailyReport(
        Guid reportId,
        [FromBody] ApproveReportRequest request)
    {
        var result = await _assistantService.ApproveDailyReportAsync(reportId, request.CaregiverNotes);
        return Ok(result);
    }

    /// <summary>
    /// Get daily reports for a beneficiary
    /// </summary>
    [HttpGet("daily-reports/{beneficiaryId}")]
    [Authorize(Roles = "Caregiver,Customer")]
    public async Task<ActionResult<List<DailyReportDto>>> GetDailyReports(Guid beneficiaryId, [FromQuery] int days = 7)
    {
        var result = await _assistantService.GetDailyReportsAsync(beneficiaryId, days);
        return Ok(result);
    }

    #endregion
}

#region Request Models

public class CreateCareNoteRequest
{
    public Guid BookingId { get; set; }
    public Guid BeneficiaryId { get; set; }
    public string Observation { get; set; } = string.Empty;
    public int MoodLevel { get; set; } // 1-5
}

public class CompleteActivityRequest
{
    public int EngagementRating { get; set; } // 1-5
    public string? Feedback { get; set; }
}

public class GenerateReportRequest
{
    public Guid BookingId { get; set; }
    public DateTime ReportDate { get; set; }
}

public class ApproveReportRequest
{
    public string? CaregiverNotes { get; set; }
}

#endregion
