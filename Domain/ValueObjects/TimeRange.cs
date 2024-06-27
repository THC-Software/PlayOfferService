using System.Text.Json.Serialization;

namespace PlayOfferService.Domain.ValueObjects;

public class TimeRange
{
    [JsonPropertyName("start")]
    public DateTime Start { get; }
    [JsonPropertyName("end")]
    public DateTime End { get; }
}