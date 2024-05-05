using System.ComponentModel.DataAnnotations.Schema;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;

namespace PlayOfferService.Models;

public class Member
{
    public Guid Id { get; set; }
    public Club Club { get; set; }
    
    public bool IsLocked { get; set; }

    public void Apply(List<BaseEvent> baseEvents)
    {
        if (Id == Guid.Empty && baseEvents.First().EventType != EventType.MEMBER_ACCOUNT_CREATED)
        {
            throw new ArgumentException("First Member event must be of type "
                                        + nameof(EventType.MEMBER_ACCOUNT_CREATED));
        }

        foreach (var baseEvent in baseEvents)
        {
            switch (baseEvent.EventType)
            {
                case EventType.MEMBER_ACCOUNT_CREATED:
                    Apply((MemberCreatedEvent) baseEvent.EventData);
                    break;
                case EventType.MEMBER_ACCOUNT_LOCKED:
                    Apply((MemberLockedEvent) baseEvent.EventData);
                    break;
                case EventType.MEMBER_ACCOUNT_UNLOCKED:
                    Apply((MemberUnlockedEvent) baseEvent.EventData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private void Apply(MemberCreatedEvent domainEvent)
    {
        Id = domainEvent.MemberAccountId;
        Club = domainEvent.Club;
    }
    
    private void Apply(MemberLockedEvent domainEvent)
    {
        IsLocked = true;
    }
    
    private void Apply(MemberUnlockedEvent domainEvent)
    {
        IsLocked = false;
    }
}