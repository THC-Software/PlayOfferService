using System.ComponentModel.DataAnnotations.Schema;
using PlayOfferService.Domain.Events;

namespace PlayOfferService.Domain.Models;

public class Club
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public Status Status { get; set; }

    public void Apply(List<BaseEvent> baseEvents)
    {
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
                case EventType.TENNIS_CLUB_DELETED:
                    Apply((ClubDeletedEvent) baseEvent.EventData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(baseEvent.EventType)} is not supported for the entity Club!");
            }
        }
    }
    
    private void Apply(ClubCreatedEvent domainEvent)
    {
        Id = domainEvent.TennisClubId.Id;
    }
    
    private void Apply(ClubLockedEvent domainEvent)
    {
        Status = Status.LOCKED;
    }
    
    private void Apply(ClubUnlockedEvent domainEvent)
    {
        Status = Status.ACTIVE;
    }
    
    private void Apply(ClubDeletedEvent domainEvent)
    {
        Status = Status.DELETED;
    }
}