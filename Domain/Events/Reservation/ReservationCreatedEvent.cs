using System.Text.Json.Serialization;

namespace PlayOfferService.Domain.Events.Reservation;

public class ReservationCreatedEvent : DomainEvent
{
    [JsonPropertyName("start")]
    public DateTime Start { get; set; }
    [JsonPropertyName("end")]
    public DateTime End { get; set; }
    [JsonPropertyName("reservantId")]
    public Guid? ReservantId { get; set; }
    [JsonPropertyName("tournamentId")]
    public Guid? TournamentId { get; set; }
    [JsonPropertyName("participantIds")]
    public List<Guid>? ParticipantIds { get; set; }
    [JsonPropertyName("courtIds")]
    public List<Guid> CourtIds { get; set; }
    
    public ReservationCreatedEvent(){}
}

