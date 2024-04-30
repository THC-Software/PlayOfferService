using System.ComponentModel.DataAnnotations.Schema;

namespace PlayOfferService.Domain.Events;

public abstract class BaseEvent<T>(
    Guid eventId, Guid entityId, EventType eventType, EntityType entityType, DateTime createdAt, T domainDomainEvent)
    where T : IDomainEvent
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid EventId { get; set; } = eventId;
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid EntityId { get; set; } = entityId;
    public EventType EventType { get; set; } = eventType;
    public EntityType EntityType { get; set; } = entityType;
    public DateTime CreatedAt { get; set; } = createdAt;
    public T DomainEvent { get; set; } = domainDomainEvent;
}