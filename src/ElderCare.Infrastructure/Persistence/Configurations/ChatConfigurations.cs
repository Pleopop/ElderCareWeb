using ElderCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElderCare.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Title).HasMaxLength(200);
        
        builder.HasIndex(c => c.Type);
        builder.HasIndex(c => c.BookingId);
        builder.HasIndex(c => c.LastMessageAt);
        builder.HasIndex(c => c.IsArchived);
        
        builder.HasOne(c => c.Booking)
            .WithMany()
            .HasForeignKey(c => c.BookingId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.ToTable("ConversationParticipants");
        builder.HasKey(cp => cp.Id);
        
        builder.HasIndex(cp => cp.UserId);
        builder.HasIndex(cp => cp.ConversationId);
        builder.HasIndex(cp => new { cp.ConversationId, cp.UserId }).IsUnique();
        
        builder.HasOne(cp => cp.Conversation)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(cp => cp.User)
            .WithMany()
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Content).IsRequired().HasMaxLength(4000);
        builder.Property(m => m.AttachmentUrl).HasMaxLength(500);
        builder.Property(m => m.AttachmentType).HasMaxLength(50);
        
        builder.HasIndex(m => m.ConversationId);
        builder.HasIndex(m => m.SenderId);
        builder.HasIndex(m => new { m.ConversationId, m.SentAt });
        builder.HasIndex(m => m.Status);
        
        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
