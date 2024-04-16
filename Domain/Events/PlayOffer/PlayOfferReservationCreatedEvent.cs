using System.Text.Json.Serialization;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(PlayOfferReservationCreatedEvent), typeDiscriminator: "PLAYOFFER_RESERVATION_CREATED")]
public class PlayOfferReservationCreatedEvent(
    Guid eventId,
    Guid entityId,
    EventType eventType,
    EntityType entityType,
    DateTime createdAt,
    Reservation reservation)
    : BaseEvent(eventId, entityId, eventType, entityType, createdAt)
{
    public Reservation Reservation { get; set; } = reservation;
}