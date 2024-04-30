using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlayOfferService.Domain.Events;

namespace PlayOfferService.Repositories;

public class BaseEventConfiguration : IEntityTypeConfiguration<BaseEvent<IDomainEvent>>
{
    public void Configure(EntityTypeBuilder<BaseEvent<IDomainEvent>> builder)
    {
        builder.ToTable("Events");
        builder.HasKey(e => e.EventId);
        
        builder.Property(e => e.EventId)
            .IsRequired()
            .ValueGeneratedNever();
        
        
        builder.Property(e => e.EntityId)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
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

        builder.Property(e => e.DomainEvent)
            .HasConversion<string>(
                e => JsonSerializer.Serialize(e, JsonSerializerOptions.Default),
                e => JsonSerializer.Deserialize<IDomainEvent>(e, JsonSerializerOptions.Default)
            )
            .IsRequired();
    }
}