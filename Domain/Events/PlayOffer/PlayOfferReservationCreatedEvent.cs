using PlayOfferService.Models;

namespace PlayOfferService.Domain.Events;

public class PlayOfferReservationCreatedEvent : IDomainEvent
{
    public Reservation Reservation { get; set; }
    
    public PlayOfferReservationCreatedEvent(){}
    
    public PlayOfferReservationCreatedEvent(
        Reservation reservation)
    {
        Reservation = reservation;
    }
}

