namespace PlayOfferService.Domain.Events.PlayOffer;

public class PlayOfferReservationAddedEvent : DomainEvent
{
    public Guid ReservationId { get; set; }
}