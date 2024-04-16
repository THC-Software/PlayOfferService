using System.Text.Json.Serialization;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(PlayOfferCancelledEvent), typeDiscriminator: "PLAYOFFER_CANCELLED")]
public class PlayOfferCancelledEvent(
    Guid eventId,
    Guid entityId,
    EventType eventType,
    EntityType entityType,
    DateTime createdAt)
    : BaseEvent(eventId, entityId, eventType, entityType, createdAt)
{
}