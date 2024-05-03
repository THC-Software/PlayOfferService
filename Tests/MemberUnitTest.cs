using NUnit.Framework;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Models;

namespace PlayOfferService.Tests;

[TestFixture]
public class MemberUnitTest
{
    [Test]
    public void ApplyMemberCreationEventTest()
    {
        // Given
        var memberId = Guid.NewGuid();
        var clubId = Guid.NewGuid();
        var memberCreationEvent = new BaseEvent<IDomainEvent>
        {
            EntityId = memberId,
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_ACCOUNT_CREATED,
            EventData = new MemberCreatedEvent
            {
                MemberAccountId = memberId,
                Club = new Club
                {
                    Id = clubId
                }
            }
        };
        
        // When
        var member = new Member();
        member.Apply(memberCreationEvent);
        
        // Then
        Assert.That(member.Id, Is.EqualTo(memberId));
        Assert.That(member.Club.Id, Is.EqualTo(clubId));
        Assert.That(member.IsLocked, Is.False);
    }
    
    [Test]
    public void ApplyMemberLockEventTest()
    {
        // Given
        var member = new Member
        {
            Id = Guid.NewGuid(),
            IsLocked = false
        };
        var memberLockEvent = new BaseEvent<IDomainEvent>
        {
            EntityId = member.Id,
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_ACCOUNT_LOCKED,
            EventData = new MemberLockedEvent()
        };
        
        // When
        member.Apply(memberLockEvent);
        
        // Then
        Assert.That(member.IsLocked, Is.True);
    }
    
    [Test]
    public void ApplyMemberUnlockEventTest()
    {
        // Given
        var member = new Member
        {
            Id = Guid.NewGuid(),
            IsLocked = true
        };
        var memberUnlockEvent = new BaseEvent<IDomainEvent>
        {
            EntityId = member.Id,
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_ACCOUNT_UNLOCKED,
            EventData = new MemberUnlockedEvent()
        };
        
        // When
        member.Apply(memberUnlockEvent);
        
        // Then
        Assert.That(member.IsLocked, Is.False);
    }
}