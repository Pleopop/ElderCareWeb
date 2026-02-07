using ElderCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElderCare.Infrastructure.Persistence.Configurations;

public class CareNoteConfiguration : IEntityTypeConfiguration<CareNote>
{
    public void Configure(EntityTypeBuilder<CareNote> builder)
    {
        builder.ToTable("CareNotes");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Observation).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.AiMoodAnalysis).HasMaxLength(1000);
        builder.Property(c => c.SentimentScore).HasPrecision(3, 2);
        
        builder.HasIndex(c => c.BeneficiaryId);
        builder.HasIndex(c => c.CaregiverId);
        builder.HasIndex(c => c.ObservedAt);
        builder.HasIndex(c => c.RequiresAttention);
        
        builder.HasOne(c => c.Booking)
            .WithMany()
            .HasForeignKey(c => c.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(c => c.Caregiver)
            .WithMany()
            .HasForeignKey(c => c.CaregiverId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(c => c.Beneficiary)
            .WithMany()
            .HasForeignKey(c => c.BeneficiaryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ActivitySuggestionConfiguration : IEntityTypeConfiguration<ActivitySuggestion>
{
    public void Configure(EntityTypeBuilder<ActivitySuggestion> builder)
    {
        builder.ToTable("ActivitySuggestions");
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Description).IsRequired().HasMaxLength(1000);
        builder.Property(a => a.AiReasoning).HasMaxLength(500);
        builder.Property(a => a.ConfidenceScore).HasPrecision(3, 2);
        builder.Property(a => a.CaregiverFeedback).HasMaxLength(1000);
        
        builder.HasIndex(a => a.BeneficiaryId);
        builder.HasIndex(a => a.Category);
        builder.HasIndex(a => a.IsCompleted);
        builder.HasIndex(a => a.GeneratedAt);
        
        builder.HasOne(a => a.Beneficiary)
            .WithMany()
            .HasForeignKey(a => a.BeneficiaryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class DailyReportConfiguration : IEntityTypeConfiguration<DailyReport>
{
    public void Configure(EntityTypeBuilder<DailyReport> builder)
    {
        builder.ToTable("DailyReports");
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Summary).IsRequired().HasMaxLength(2000);
        builder.Property(d => d.HealthNotes).HasMaxLength(1000);
        builder.Property(d => d.BehaviorNotes).HasMaxLength(1000);
        builder.Property(d => d.AiInsights).HasMaxLength(1000);
        builder.Property(d => d.CaregiverNotes).HasMaxLength(2000);
        
        builder.HasIndex(d => d.BeneficiaryId);
        builder.HasIndex(d => d.ReportDate);
        builder.HasIndex(d => new { d.BookingId, d.ReportDate }).IsUnique();
        builder.HasIndex(d => d.ViewedByCustomer);
        
        builder.HasOne(d => d.Booking)
            .WithMany()
            .HasForeignKey(d => d.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(d => d.Caregiver)
            .WithMany()
            .HasForeignKey(d => d.CaregiverId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(d => d.Beneficiary)
            .WithMany()
            .HasForeignKey(d => d.BeneficiaryId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(d => d.Customer)
            .WithMany()
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
