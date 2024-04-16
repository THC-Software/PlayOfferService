using System.Text.Json.Serialization;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(PlayOfferJoinedEvent), typeDiscriminator: "PLAYOFFER_JOINED")]
public class PlayOfferJoinedEvent(
    Guid eventId,
    Guid entityId,
    EventType eventType,
    EntityType entityType,
    DateTime createdAt,
    Member opponent, 
    DateTime acceptedStartTime)
    : BaseEvent(eventId, entityId, eventType, entityType, createdAt)
{
    public Member Opponent { get; set; } = opponent;
    public DateTime AcceptedStartTime { get; set; } = acceptedStartTime;
}