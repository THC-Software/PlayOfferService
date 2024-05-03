using NUnit.Framework;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Models;

namespace PlayOfferService.Tests;

[TestFixture]
public class MemberUnitTest
{
    [Test]
    public void ApplyMemberCreatedEventTest()
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
        member.Apply([memberCreationEvent]);
        
        // Then
        Assert.That(member.Id, Is.EqualTo(memberId));
        Assert.That(member.Club.Id, Is.EqualTo(clubId));
        Assert.That(member.IsLocked, Is.False);
    }
    
    [Test]
    public void ApplyMemberLockEventTest()
    {
        // Given
        var memberEvents = new List<BaseEvent<IDomainEvent>>
        {
            new()
            {
                EventType = EventType.MEMBER_ACCOUNT_CREATED,
                EventData = new MemberCreatedEvent()
            },
            new()
            {
                EventType = EventType.MEMBER_ACCOUNT_LOCKED,
                EventData = new MemberLockedEvent()
            }
        };
        
        // When
        var member = new Member();
        member.Apply(memberEvents);
        
        // Then
        Assert.That(member.IsLocked, Is.True);
    }
    
    [Test]
    public void ApplyMemberUnlockEventTest()
    {
        // Given
        var memberEvents = new List<BaseEvent<IDomainEvent>>
        {
            new()
            {
                EventType = EventType.MEMBER_ACCOUNT_CREATED,
                EventData = new MemberCreatedEvent()
            },
            new()
            {
                EventType = EventType.MEMBER_ACCOUNT_LOCKED,
                EventData = new MemberLockedEvent()
            },
            new()
            {
                EventType = EventType.MEMBER_ACCOUNT_UNLOCKED,
                EventData = new MemberUnlockedEvent()
            }
        };
        
        // When
        var member = new Member();
        member.Apply(memberEvents);
        
        // Then
        Assert.That(member.IsLocked, Is.False);
    }
}