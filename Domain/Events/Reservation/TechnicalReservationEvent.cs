using MediatR;

namespace PlayOfferService.Domain.Events.Reservation;

public class TechnicalReservationEvent : BaseEvent, IRequest
{
    public TechnicalReservationEvent() { }
}