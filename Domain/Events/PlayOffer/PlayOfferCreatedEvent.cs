using System.Text.Json.Serialization;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(PlayOfferCreatedEvent), typeDiscriminator: "PLAYOFFER_CREATED")]
public class PlayOfferCreatedEvent(
    Guid eventId,
    Guid entityId,
    EventType eventType,
    EntityType entityType,
    DateTime createdAt,
    Club club,
    Member creator,
    DateTime proposedStartTime,
    DateTime proposedEndTime)
    : BaseEvent(eventId, entityId, eventType, entityType, createdAt)
{
    public Club Club { get; set; } = club;
    public Member Creator { get; set; } = creator;
    public DateTime ProposedStartTime { get; set; } = proposedStartTime;
    public DateTime ProposedEndTime { get; set; } = proposedEndTime;
}