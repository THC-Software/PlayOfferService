using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;

namespace PlayOfferService.Models;

public class Member
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    
    public Status Status { get; set; }

    public void Apply(List<BaseEvent> baseEvents)
    {
        foreach (var baseEvent in baseEvents)
        {
            switch (baseEvent.EventType)
            {
                case EventType.MEMBER_REGISTERED:
                    Apply((MemberCreatedEvent) baseEvent.EventData);
                    break;
                case EventType.MEMBER_LOCKED:
                    Apply((MemberLockedEvent) baseEvent.EventData);
                    break;
                case EventType.MEMBER_UNLOCKED:
                    Apply((MemberUnlockedEvent) baseEvent.EventData);
                    break;
                case EventType.MEMBER_DELETED:
                    Apply((MemberDeletedEvent) baseEvent.EventData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(baseEvent.EventType)} is not supported for the entity Member!");
            }
        }
    }
    
    private void Apply(MemberCreatedEvent domainEvent)
    {
        Id = domainEvent.MemberId.Id;
        ClubId = domainEvent.TennisClubId.Id;
        Status = domainEvent.Status;
    }
    
    private void Apply(MemberLockedEvent domainEvent)
    {
        Status = Status.LOCKED;
    }
    
    private void Apply(MemberUnlockedEvent domainEvent)
    {
        Status = Status.ACTIVE;
    }
    
    private void Apply(MemberDeletedEvent domainEvent)
    {
        Status = Status.DELETED;
    }
}