using System.ComponentModel.DataAnnotations.Schema;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Reservation;

namespace PlayOfferService.Domain.Models;

public class Reservation
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public List<Guid> CourtIds { get; set; }
    public Guid? ReservantId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<Guid> ParticipantsIds { get; set; }
    public bool IsCancelled { get; set; }
    
    public void Apply (List<BaseEvent> baseEvents)
    {
        foreach (var baseEvent in baseEvents)
        {
            switch (baseEvent.EventType)
            {
                case EventType.ReservationCreatedEvent:
                    ApplyReservationCreatedEvent(baseEvent);
                    break;
                case EventType.ReservationCancelledEvent:
                    ApplyReservationCancelledEvent();
                    break;
            }
        }
    }

    private void ApplyReservationCancelledEvent()
    {
        IsCancelled = true;
    }

    private void ApplyReservationCreatedEvent(BaseEvent baseEvent)
    {
        var domainEvent = (ReservationCreatedEvent) baseEvent.EventData;
        
        Id = baseEvent.EntityId;
        CourtIds = domainEvent.CourtIds;
        ReservantId = domainEvent.ReservantId;
        StartTime = domainEvent.Start;
        EndTime = domainEvent.End;
        ParticipantsIds = domainEvent.ParticipantIds;
        IsCancelled = false;
    }
}