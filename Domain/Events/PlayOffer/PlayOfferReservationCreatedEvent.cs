using PlayOfferService.Domain.Models;

namespace PlayOfferService.Domain.Events.PlayOffer;

public class PlayOfferReservationCreatedEvent : DomainEvent
{
    public Reservation Reservation { get; set; }
    
    public PlayOfferReservationCreatedEvent(){}
    
    public PlayOfferReservationCreatedEvent(
        Reservation reservation)
    {
        Reservation = reservation;
    }
}

