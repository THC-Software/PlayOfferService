namespace PlayOfferService.Domain.Events;

public abstract class BaseEvent(Guid eventId, Guid entityId, EventType eventType, EntityType entityType, DateTime createdAt)
{
    public Guid EventId { get; set; } = eventId;
    public Guid EntityId { get; set; } = entityId;
    public EventType EventType { get; set; } = eventType;
    public EntityType EntityType { get; set; } = entityType;
    public DateTime CreatedAt { get; set; } = createdAt;
}