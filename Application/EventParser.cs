using System.Text.Json;
using System.Text.Json.Nodes;
using PlayOfferService.Domain.Events;

namespace PlayOfferService.Application;

public class EventParser
{
    public static BaseEvent ParseEvent(JsonNode jsonEvent)
    {
        var originalEventData = JsonNode.Parse(jsonEvent["eventData"].GetValue<string>());
        
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
            Timestamp = DateTime.Parse(jsonEvent["timestamp"].GetValue<string>()).ToUniversalTime(),
            EntityId = Guid.Parse(jsonEvent["entityId"].GetValue<string>()),
            EntityType = (EntityType)Enum.Parse(typeof(EntityType), jsonEvent["entityType"].GetValue<string>()),
            EventData = JsonSerializer.Deserialize<IDomainEvent>(newEventData, JsonSerializerOptions.Default),
        };
    }
}