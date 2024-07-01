using System.Text.Json;
using System.Text.Json.Nodes;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Reservation;

namespace PlayOfferService.Application;

public class EventParser
{
    public static T ParseEvent<T>(JsonNode jsonEvent) where T : BaseEvent, new()
    {
        JsonNode? originalEventData = null;
        DateTime timestamp = DateTime.UtcNow;
        
        // If eventData is JsonValue (escaped as string), parse it
        if (jsonEvent["eventData"] is JsonValue)
        {
            originalEventData = JsonNode.Parse(jsonEvent["eventData"].GetValue<string>());
            timestamp = DateTime.Parse(jsonEvent["timestamp"].GetValue<string>()).ToUniversalTime();
        }
        else // If eventData is already a JsonNode, just use it --> court_service
        {
            originalEventData = jsonEvent["eventData"];
            timestamp = DateTimeOffset.FromUnixTimeMilliseconds(jsonEvent["timestamp"]["$date"].GetValue<long>())
                .UtcDateTime;
        }

        // We need to add the eventType to the event data so we can deserialize it correctly
        // Since the discriminator needs to be at the first position in the JSON object, we need to create a new object
        // because JsonNode doesn't allow us to insert at the beginning of the object
        
        var newEventData = new JsonObject();
        newEventData["eventType"] = jsonEvent["eventType"].GetValue<string>();
        foreach (var kvp in originalEventData.AsObject())
        {
            if (kvp.Value == null)
            {
                newEventData[kvp.Key] = null;
                continue;
            }
            
            if (kvp.Value is JsonObject && kvp.Value?["$date"] != null)
            {
                newEventData[kvp.Key] = DateTimeOffset.FromUnixTimeMilliseconds(kvp.Value["$date"]
                    .GetValue<long>()).UtcDateTime;
            }
            else
            {
                newEventData[kvp.Key] = kvp.Value.DeepClone();
            }
        }
        
        Guid? correlationId = null;
        if (jsonEvent["correlationId"] != null)
        {
            correlationId = Guid.Parse(jsonEvent["correlationId"].GetValue<string>());
        }
        
        
        return new T
        {
            EventId = Guid.Parse(jsonEvent["eventId"].GetValue<string>()),
            EventType = (EventType)Enum.Parse(typeof(EventType), jsonEvent["eventType"].GetValue<string>()),
            Timestamp = timestamp,
            EntityId = Guid.Parse(jsonEvent["entityId"].GetValue<string>()),
            EntityType = (EntityType)Enum.Parse(typeof(EntityType), jsonEvent["entityType"].GetValue<string>()),
            EventData = JsonSerializer.Deserialize<DomainEvent>(newEventData, JsonSerializerOptions.Default),
            CorrelationId = correlationId
        };
    }
}