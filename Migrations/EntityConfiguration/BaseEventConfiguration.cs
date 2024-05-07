using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlayOfferService.Domain.Events;

namespace PlayOfferService.Repositories;

public class BaseEventConfiguration : IEntityTypeConfiguration<BaseEvent>
{
    public void Configure(EntityTypeBuilder<BaseEvent> builder)
    {
        builder.ToTable("events");
        builder.HasKey(e => e.EventId);
        
        builder.Property(e => e.EventId)
            .IsRequired()
            .ValueGeneratedNever();
        
        
        builder.Property(e => e.EntityId)
            .IsRequired();
        builder.Property(e => e.Timestamp)
            .IsRequired();
        
        builder.Property(e => e.EventType)
            .HasConversion(
                e => e.ToString(), 
                e => (EventType)Enum.Parse(typeof(EventType), e)
                )
            .IsRequired();
        builder.Property(e => e.EntityType)
            .HasConversion(e => e.ToString(),
                e => (EntityType)Enum.Parse(typeof(EntityType), e)
            )
            .IsRequired();

        builder.Property(e => e.EventData)
            .HasConversion<string>(
                e => JsonSerializer.Serialize<object>(e, JsonSerializerOptions.Default),
                e => JsonSerializer.Deserialize<IDomainEvent>(e, JsonSerializerOptions.Default)
            )
            .IsRequired();
    }
}