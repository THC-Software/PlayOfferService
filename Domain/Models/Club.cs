using System.ComponentModel.DataAnnotations.Schema;
using PlayOfferService.Domain.Events;

namespace PlayOfferService.Models;

public class Club
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public bool IsLocked { get; set; }

    public void Apply(List<BaseEvent<IDomainEvent>> baseEvents)
    {
        if (Id == Guid.Empty && baseEvents.First().EventType != EventType.TENNIS_CLUB_REGISTERED)
        {
            throw new ArgumentException("First Club event must be of type "
                                        +nameof(EventType.TENNIS_CLUB_REGISTERED));
        }
        
        foreach (var baseEvent in baseEvents)
        {
            switch (baseEvent.EventType)
            {
                case EventType.TENNIS_CLUB_REGISTERED:
                    Apply((ClubCreatedEvent) baseEvent.EventData);
                    break;
                case EventType.TENNIS_CLUB_LOCKED:
                    Apply((ClubLockedEvent) baseEvent.EventData);
                    break;
                case EventType.TENNIS_CLUB_UNLOCKED:
                    Apply((ClubUnlockedEvent) baseEvent.EventData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private void Apply(ClubCreatedEvent domainEvent)
    {
        Id = domainEvent.TennisClubId;
    }
    
    private void Apply(ClubLockedEvent domainEvent)
    {
        IsLocked = true;
    }
    
    private void Apply(ClubUnlockedEvent domainEvent)
    {
        IsLocked = false;
    }
}