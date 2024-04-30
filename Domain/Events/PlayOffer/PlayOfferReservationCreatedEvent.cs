using System.Text.Json.Serialization;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(PlayOfferReservationCreatedEvent), typeDiscriminator: "PLAYOFFER_RESERVATION_CREATED")]
public class PlayOfferReservationCreatedEvent(Reservation reservation)
    : IDomainEvent
{
    public Reservation Reservation { get; set; } = reservation;
}

