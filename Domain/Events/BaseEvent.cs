using System.ComponentModel.DataAnnotations.Schema;

namespace PlayOfferService.Domain.Events;

public class BaseEvent<T> where T : IDomainEvent
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid EventId { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid EntityId { get; set; }
    public EventType EventType { get; set; }
    public EntityType EntityType { get; set; }
    public DateTime CreatedAt { get; set; }
    public T DomainEvent { get; set; }
    
    public BaseEvent(){}
    
    public BaseEvent(
        Guid eventId,
        Guid entityId,
        EventType eventType,
        EntityType entityType,
        DateTime createdAt,
        T domainEvent)
    {
        EventId = eventId;
        EntityId = entityId;
        EventType = eventType;
        EntityType = entityType;
        CreatedAt = createdAt;
        DomainEvent = domainEvent;
    }
}