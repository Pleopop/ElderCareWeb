using ElderCare.Domain.Enums;

namespace ElderCare.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string? SecurityPin { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public UserRole Role { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public bool IsPhoneVerified { get; set; } = false;
    
    // Navigation properties
    public CustomerProfile? CustomerProfile { get; set; }
    public CaregiverProfile? CaregiverProfile { get; set; }
    public Wallet? Wallet { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;
    
    public User User { get; set; } = null!;
}
