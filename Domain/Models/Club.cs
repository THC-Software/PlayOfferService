using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Club;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayOfferService.Domain.Models;

public class Club
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Status Status { get; set; }

    public void Apply(List<BaseEvent> baseEvents)
    {
        foreach (var baseEvent in baseEvents)
        {
            switch (baseEvent.EventType)
            {
                case EventType.TENNIS_CLUB_REGISTERED:
                    ApplyClubCreatedEvent((ClubCreatedEvent)baseEvent.EventData);
                    break;
                case EventType.TENNIS_CLUB_LOCKED:
                    ApplyClubLockedEvent();
                    break;
                case EventType.TENNIS_CLUB_UNLOCKED:
                    ApplyClubUnlockedEvent();
                    break;
                case EventType.TENNIS_CLUB_DELETED:
                    ApplyClubDeletedEvent();
                    break;
                case EventType.TENNIS_CLUB_NAME_CHANGED:
                    ApplyClubNameChangedEvent((ClubNameChangedEvent)baseEvent.EventData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(baseEvent.EventType)} is not supported for the entity Club!");
            }
        }
    }

    private void ApplyClubCreatedEvent(ClubCreatedEvent domainEvent)
    {
        Id = domainEvent.TennisClubId.Id;
        Name = domainEvent.Name;
    }

    private void ApplyClubLockedEvent()
    {
        Status = Status.LOCKED;
    }

    private void ApplyClubUnlockedEvent()
    {
        Status = Status.ACTIVE;
    }

    private void ApplyClubDeletedEvent()
    {
        Status = Status.DELETED;
    }

    private void ApplyClubNameChangedEvent(ClubNameChangedEvent domainEvent)
    {
        Name = domainEvent.Name;
    }
}