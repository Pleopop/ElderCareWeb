namespace ElderCare.Domain.Entities;

/// <summary>
/// AI-assisted daily care report for customers (family members)
/// Summarizes caregiver observations, activities, and beneficiary's condition
/// </summary>
public class DailyReport : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid CaregiverId { get; set; }
    public Guid BeneficiaryId { get; set; }
    public Guid CustomerId { get; set; }
    
    public DateTime ReportDate { get; set; }
    
    // Report content
    public string Summary { get; set; } = string.Empty; // AI-generated summary
    public Enums.MoodLevel AverageMood { get; set; }
    public List<string>? ActivitiesCompleted { get; set; } // JSON array
    public List<string>? MealsConsumed { get; set; } // JSON array
    public string? HealthNotes { get; set; }
    public string? BehaviorNotes { get; set; }
    
    // AI insights
    public string? AiInsights { get; set; } // AI-generated insights
    public List<string>? PositiveHighlights { get; set; } // JSON: good moments
    public List<string>? AreasOfConcern { get; set; } // JSON: things to watch
    
    // Caregiver input
    public string? CaregiverNotes { get; set; }
    public bool CaregiverApproved { get; set; }
    public DateTime? ApprovedAt { get; set; }
    
    // Customer interaction
    public bool ViewedByCustomer { get; set; }
    public DateTime? ViewedAt { get; set; }
    
    public DateTime GeneratedAt { get; set; }
    
    // Navigation properties
    public Booking Booking { get; set; } = null!;
    public Caregiver Caregiver { get; set; } = null!;
    public Beneficiary Beneficiary { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
}
