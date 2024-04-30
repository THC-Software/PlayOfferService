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
    public DateTime Timestamp { get; set; }
    public T EventData { get; set; }
    
    public BaseEvent(){}
    
    public BaseEvent(
        Guid eventId,
        Guid entityId,
        EventType eventType,
        EntityType entityType,
        DateTime timestamp,
        T eventData)
    {
        EventId = eventId;
        EntityId = entityId;
        EventType = eventType;
        EntityType = entityType;
        Timestamp = timestamp;
        EventData = eventData;
    }
}