using ElderCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElderCare.Infrastructure.Persistence;

public class ElderCareDbContext : DbContext
{
    public ElderCareDbContext(DbContextOptions<ElderCareDbContext> options) : base(options)
    {
    }

    // Identity
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    
    // Customer
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();
    public DbSet<BeneficiaryPreference> BeneficiaryPreferences => Set<BeneficiaryPreference>();
    
    // Caregiver
    public DbSet<Caregiver> Caregivers => Set<Caregiver>();
    public DbSet<CaregiverSkill> CaregiverSkills => Set<CaregiverSkill>();
    public DbSet<CaregiverAvailability> CaregiverAvailabilities => Set<CaregiverAvailability>();
    public DbSet<PersonalityAssessment> PersonalityAssessments => Set<PersonalityAssessment>();
    
    // Booking
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<LocationLog> LocationLogs => Set<LocationLog>();
    
    // Dispute
    public DbSet<Dispute> Disputes => Set<Dispute>();
    public DbSet<DisputeEvidence> DisputeEvidences => Set<DisputeEvidence>();
    
    // Payment
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    
    // AI Matching
    public DbSet<MatchingResult> MatchingResults => Set<MatchingResult>();
    
    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();
    
    // AI Caregiver Assistant
    public DbSet<CareNote> CareNotes => Set<CareNote>();
    public DbSet<ActivitySuggestion> ActivitySuggestions => Set<ActivitySuggestion>();
    public DbSet<DailyReport> DailyReports => Set<DailyReport>();
    
    // Chat System
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<Message> Messages => Set<Message>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElderCareDbContext).Assembly);
        
        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Customer>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Caregiver>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Beneficiary>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Booking>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}
