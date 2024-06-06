using System.Text.Json.Serialization;

namespace PlayOfferService.Domain.ValueObjects;

public class Restriction
{
    [JsonPropertyName("weekday")]
    public Weekday Weekday { get; set; }
    [JsonPropertyName("timeRanges")]
    public List<TimeRange> TimeRanges { get; set; }
}