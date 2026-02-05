using ElderCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElderCare.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(512);
        builder.Property(u => u.SecurityPin).HasMaxLength(6);
        
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.PhoneNumber).IsUnique();
        
        builder.HasOne(u => u.CustomerProfile)
            .WithOne(c => c.User)
            .HasForeignKey<CustomerProfile>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(u => u.CaregiverProfile)
            .WithOne(c => c.User)
            .HasForeignKey<CaregiverProfile>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(u => u.Wallet)
            .WithOne(w => w.User)
            .HasForeignKey<Wallet>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class CustomerProfileConfiguration : IEntityTypeConfiguration<CustomerProfile>
{
    public void Configure(EntityTypeBuilder<CustomerProfile> builder)
    {
        builder.ToTable("CustomerProfiles");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.FullName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.EmergencyContactName).HasMaxLength(200);
        builder.Property(c => c.EmergencyContactPhone).HasMaxLength(20);
        
        builder.HasIndex(c => c.UserId).IsUnique();
    }
}

public class CaregiverProfileConfiguration : IEntityTypeConfiguration<CaregiverProfile>
{
    public void Configure(EntityTypeBuilder<CaregiverProfile> builder)
    {
        builder.ToTable("CaregiverProfiles");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.FullName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.IdentityNumber).HasMaxLength(50);
        builder.Property(c => c.IdentityImageUrl).HasMaxLength(500);
        builder.Property(c => c.IdentityBackImageUrl).HasMaxLength(500);
        builder.Property(c => c.SelfieUrl).HasMaxLength(500);
        builder.Property(c => c.CriminalRecordUrl).HasMaxLength(500);
        builder.Property(c => c.Bio).HasMaxLength(2000);
        builder.Property(c => c.PersonalityType).HasMaxLength(50);
        builder.Property(c => c.HourlyRate).HasPrecision(18, 2);
        builder.Property(c => c.Address).HasMaxLength(500);
        
        builder.HasIndex(c => c.UserId).IsUnique();
        builder.HasIndex(c => c.VerificationStatus);
    }
}

public class BeneficiaryConfiguration : IEntityTypeConfiguration<Beneficiary>
{
    public void Configure(EntityTypeBuilder<Beneficiary> builder)
    {
        builder.ToTable("Beneficiaries");
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.FullName).IsRequired().HasMaxLength(200);
        builder.Property(b => b.MedicalConditions).HasMaxLength(2000);
        builder.Property(b => b.Allergies).HasMaxLength(1000);
        builder.Property(b => b.PersonalityTraits).HasMaxLength(1000);
        builder.Property(b => b.Hobbies).HasMaxLength(1000);
        builder.Property(b => b.DailyRoutine).HasMaxLength(2000);
        
        builder.HasOne(b => b.CustomerProfile)
            .WithMany(c => c.Beneficiaries)
            .HasForeignKey(b => b.CustomerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.ServiceLocation).IsRequired().HasMaxLength(500);
        builder.Property(b => b.TotalAmount).HasPrecision(18, 2);
        builder.Property(b => b.EscrowAmount).HasPrecision(18, 2);
        builder.Property(b => b.CommissionAmount).HasPrecision(18, 2);
        builder.Property(b => b.SpecialRequirements).HasMaxLength(2000);
        builder.Property(b => b.CancellationReason).HasMaxLength(1000);
        builder.Property(b => b.CheckInPhotoUrl).HasMaxLength(500);
        builder.Property(b => b.CheckOutNotes).HasMaxLength(2000);
        
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.ScheduledStartTime);
        builder.HasIndex(b => new { b.CustomerProfileId, b.Status });
        builder.HasIndex(b => new { b.CaregiverProfileId, b.Status });
        
        builder.HasOne(b => b.CustomerProfile)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CustomerProfileId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(b => b.CaregiverProfile)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CaregiverProfileId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(b => b.Beneficiary)
            .WithMany(ben => ben.Bookings)
            .HasForeignKey(b => b.BeneficiaryId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(b => b.Review)
            .WithOne(r => r.Booking)
            .HasForeignKey<Review>(r => r.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(b => b.Dispute)
            .WithOne(d => d.Booking)
            .HasForeignKey<Dispute>(d => d.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Comment).HasMaxLength(2000);
        
        builder.HasIndex(r => r.CaregiverId);
        builder.HasIndex(r => r.BookingId).IsUnique();
    }
}

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Balance).HasPrecision(18, 2);
        builder.Property(w => w.EscrowBalance).HasPrecision(18, 2);
        
        builder.HasIndex(w => w.UserId).IsUnique();
    }
}

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Amount).HasPrecision(18, 2);
        builder.Property(t => t.Description).HasMaxLength(1000);
        
        builder.HasIndex(t => t.WalletId);
        builder.HasIndex(t => t.RelatedBookingId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.CreatedAt);
        
        builder.HasOne(t => t.Wallet)
            .WithMany(w => w.Transactions)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(t => t.RelatedBooking)
            .WithMany(b => b.Transactions)
            .HasForeignKey(t => t.RelatedBookingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class LocationLogConfiguration : IEntityTypeConfiguration<LocationLog>
{
    public void Configure(EntityTypeBuilder<LocationLog> builder)
    {
        builder.ToTable("LocationLogs");
        builder.HasKey(l => l.Id);
        
        builder.HasIndex(l => l.BookingId);
        builder.HasIndex(l => l.Timestamp);
        builder.HasIndex(l => new { l.BookingId, l.Timestamp });
        
        builder.HasOne(l => l.Booking)
            .WithMany(b => b.LocationLogs)
            .HasForeignKey(l => l.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(n => n.Id);
        
        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
        builder.Property(n => n.ActionUrl).HasMaxLength(500);
        builder.Property(n => n.RelatedEntityType).HasMaxLength(50);
        
        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IsRead);
        builder.HasIndex(n => n.CreatedAt);
        
        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class DisputeConfiguration : IEntityTypeConfiguration<Dispute>
{
    public void Configure(EntityTypeBuilder<Dispute> builder)
    {
        builder.ToTable("Disputes");
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Reason).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Description).IsRequired().HasMaxLength(2000);
        builder.Property(d => d.Resolution).HasMaxLength(2000);
        builder.Property(d => d.ResolvedBy).HasMaxLength(200);
        
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => d.BookingId).IsUnique();
    }
}
