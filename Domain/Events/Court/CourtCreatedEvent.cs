using System.Text.Json.Serialization;
using PlayOfferService.Domain.ValueObjects;

namespace PlayOfferService.Domain.Events.Court;

public class CourtCreatedEvent : DomainEvent
{
    [JsonPropertyName("clubId")]
    public Guid ClubId { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("restrictions")]
    public List<Restriction> Restrictions { get; set; }
}