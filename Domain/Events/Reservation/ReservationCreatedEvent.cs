namespace PlayOfferService.Domain.Events.Reservation;

public class ReservationCreatedEvent : DomainEvent
{
    public Guid ReservationId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public Guid? ReservantId { get; set; }
    public Guid? TournamentId { get; set; }
    public List<Guid> ParticipantIds { get; set; }
    
    public ReservationCreatedEvent(){}
}

