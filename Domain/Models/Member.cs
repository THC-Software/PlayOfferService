using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;

namespace PlayOfferService.Domain.Models;

public class Member
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }

    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? ClubName { get; set; }
    
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
                    ApplyMemberLockedEvent();
                    break;
                case EventType.MEMBER_UNLOCKED:
                    ApplyMemberUnlockedEvent();
                    break;
                case EventType.MEMBER_DELETED:
                    ApplyMemberDeletedEvent();
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
    
    private void ApplyMemberLockedEvent()
    {
        Status = Status.LOCKED;
    }
    
    private void ApplyMemberUnlockedEvent()
    {
        Status = Status.ACTIVE;
    }
    
    private void ApplyMemberDeletedEvent()
    {
        Status = Status.DELETED;
    }
}