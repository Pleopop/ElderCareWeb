using ElderCare.Domain.Enums;

namespace ElderCare.Domain.Entities;

public class Wallet : BaseEntity
{
    public Guid UserId { get; set; }
    public decimal Balance { get; set; } = 0;
    public decimal EscrowBalance { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

public class Transaction : BaseEntity
{
    public Guid WalletId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public Guid? RelatedBookingId { get; set; }
    
    // Navigation properties
    public Wallet Wallet { get; set; } = null!;
    public Booking? RelatedBooking { get; set; }
}
