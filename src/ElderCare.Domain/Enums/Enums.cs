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

public enum CognitiveStatus
{
    Normal,
    MildImpairment,
    ModerateImpairment,
    SevereImpairment
}

public enum DisputeStatus
{
    Open,
    UnderReview,
    Resolved,
    Closed
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
