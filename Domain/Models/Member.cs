using System.ComponentModel.DataAnnotations.Schema;
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
        if (Id == Guid.Empty && baseEvents.First().EventType != EventType.MEMBER_REGISTERED)
        {
            throw new ArgumentException("First Member event must be of type "
                                        + nameof(EventType.MEMBER_REGISTERED));
        }

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
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private void Apply(MemberCreatedEvent domainEvent)
    {
        Id = domainEvent.MemberId.Id;
        ClubId = domainEvent.TennisClubId.Id;
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