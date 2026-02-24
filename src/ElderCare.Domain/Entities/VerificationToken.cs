using ElderCare.Domain.Enums;

namespace ElderCare.Domain.Entities;

/// <summary>
/// Verification token for email verification, password reset, etc.
/// </summary>
public class VerificationToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public VerificationType Type { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }
    
    // Navigation property
    public User User { get; set; } = null!;
}
