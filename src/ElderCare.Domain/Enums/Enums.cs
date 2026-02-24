namespace ElderCare.Domain.Enums;

public enum UserStatus
{
    Active,
    Inactive,
    Suspended,
    Deleted
}

public enum UserRole
{
    Customer,
    Caregiver,
    Admin
}

public enum VerificationStatus
{
    Pending,
    Approved,
    Rejected
}

public enum BookingStatus
{
    Pending,
    Accepted,
    Rejected,
    InProgress,
    Completed,
    Cancelled,
    Disputed
}

public enum Gender
{
    Male,
    Female,
    Other
}

public enum MobilityLevel
{
    FullyMobile = 0,
    SlightlyLimited = 1,
    ModeratelyLimited = 2,
    SeverelyLimited = 3,
    Bedridden = 4
}

public enum CognitiveStatus
{
    Normal,
    MildImpairment,
    ModerateImpairment,
    SevereImpairment
}

public enum DisputeStatus
{
    Pending,
    UnderReview,
    AwaitingEvidence,
    Resolved,
    Rejected,
    Withdrawn
}

public enum DisputeType
{
    ServiceQuality,
    Payment,
    Safety,
    NoShow,
    Cancellation,
    Other
}

public enum EvidenceType
{
    Image,
    Document,
    Screenshot,
    Video,
    Other
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
    EscrowHold,
    EscrowRelease,
    Commission,
    Refund
}

public enum TransactionStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Cancelled = 3
}

public enum NotificationType
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3
}

public enum NotificationCategory
{
    Booking = 0,
    Payment = 1,
    Review = 2,
    Dispute = 3,
    System = 4,
    Account = 5
}

public enum NotificationPriority
{
    Low = 0,
    Medium = 1,
    High = 2
}

// AI Caregiver Assistant Enums
public enum MoodLevel
{
    VeryLow = 1,
    Low = 2,
    Neutral = 3,
    Good = 4,
    Excellent = 5
}

public enum ActivityCategory
{
    Physical,        // Exercise, walking, stretching
    Mental,          // Puzzles, reading, memory games
    Social,          // Conversation, group activities
    Creative,        // Art, music, crafts
    Recreational,    // Games, hobbies
    Spiritual,       // Prayer, meditation
    Domestic,        // Cooking, gardening
    Entertainment    // TV, movies, music listening
}

public enum DifficultyLevel
{
    VeryEasy = 1,
    Easy = 2,
    Moderate = 3,
    Challenging = 4,
    Difficult = 5
}

// Chat System Enums
public enum ConversationType
{
    OneOnOne = 0,    // 1-on-1 chat between 2 users
    Group = 1,       // Group chat (future)
    Support = 2      // Customer support chat
}


public enum MessageStatus
{
    Sent = 0,        // Message sent but not delivered
    Delivered = 1,   // Message delivered to recipient
    Read = 2         // Message read by recipient
}

// Verification
public enum VerificationType
{
    Email = 0,
    Phone = 1,
    PasswordReset = 2
}
