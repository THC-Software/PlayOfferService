using System.Text.Json.Serialization;
using PlayOfferService.Domain.ValueObjects;

namespace PlayOfferService.Domain.Events.Court;

public class CourtUpdatedEvent : DomainEvent
{
    [JsonPropertyName("courtName")]
    public string? CourtName { get; set; }
    [JsonPropertyName("restrictions")]
    public List<Restriction>? Restrictions { get; set; }
}