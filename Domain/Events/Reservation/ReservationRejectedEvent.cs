using System.Text.Json.Serialization;
using MediatR;

namespace PlayOfferService.Domain.Events.Reservation;

public class ReservationRejectedEvent : DomainEvent
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
    public Guid CourtId { get; set; }
    
    public ReservationRejectedEvent(){}
}