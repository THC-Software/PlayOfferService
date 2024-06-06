using System.ComponentModel.DataAnnotations.Schema;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Court;

namespace PlayOfferService.Domain.Models;

public class Court
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public string Name { get; set; }

    public void Apply(List<BaseEvent> baseEvents)
    {
        foreach (var baseEvent in baseEvents)
        {
            switch (baseEvent.EventType)
            {
                case EventType.CourtCreatedEvent:
                    ApplyCourtCreatedEvent(baseEvent);
                    break;
                case EventType.CourtUpdatedEvent:
                    ApplyCourtUpdatedEvent(baseEvent);
                    break;
            }
        }
    }

    private void ApplyCourtCreatedEvent(BaseEvent baseEvent)
    {
        var domainEvent = (CourtCreatedEvent)baseEvent.EventData;
        
        Id = baseEvent.EntityId;
        ClubId = domainEvent.ClubId;
        Name = domainEvent.Name;
    }
    
    private void ApplyCourtUpdatedEvent(BaseEvent baseEvent)
    {
        var domainEvent = (CourtUpdatedEvent)baseEvent.EventData;
        
        Name = domainEvent.CourtName ?? Name;
    }
}