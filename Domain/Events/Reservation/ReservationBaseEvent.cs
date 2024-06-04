using MediatR;

namespace PlayOfferService.Domain.Events.Reservation;

public class ReservationBaseEvent : BaseEvent, IRequest
{
    public ReservationBaseEvent() { }
}