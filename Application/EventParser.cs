using System.Text.Json;
using System.Text.Json.Nodes;
using PlayOfferService.Domain.Events;

namespace PlayOfferService.Application;

public class EventParser
{
    public static BaseEvent ParseEvent(JsonNode jsonEvent)
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
            newEventData[kvp.Key] = kvp.Value.DeepClone();
        }
        
        return new BaseEvent
        {
            EventId = Guid.Parse(jsonEvent["eventId"].GetValue<string>()),
            EventType = (EventType)Enum.Parse(typeof(EventType), jsonEvent["eventType"].GetValue<string>()),
            Timestamp = timestamp,
            EntityId = Guid.Parse(jsonEvent["entityId"].GetValue<string>()),
            EntityType = (EntityType)Enum.Parse(typeof(EntityType), jsonEvent["entityType"].GetValue<string>()),
            EventData = JsonSerializer.Deserialize<DomainEvent>(newEventData, JsonSerializerOptions.Default),
        };
    }
}