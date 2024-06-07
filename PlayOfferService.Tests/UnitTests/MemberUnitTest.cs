using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.ValueObjects;

namespace PlayOfferService.Tests.UnitTests;

[TestFixture]
public class MemberUnitTest
{
    [Test]
    public void ApplyMemberCreatedEventTest()
    {
        // Given
        var memberId = Guid.NewGuid();
        var clubId = Guid.NewGuid();
        var memberCreationEvent = new BaseEvent
        {
            EntityId = memberId,
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_REGISTERED,
            EventData = new MemberCreatedEvent
            {
                MemberId = new MemberId { Id = memberId },
                Name = new FullName { FirstName = "John", LastName = "Doe" },
                TennisClubId = new TennisClubId { Id = clubId }
            }
        };

        // When
        var member = new Member();
        member.Apply([memberCreationEvent]);

        // Then
        Assert.That(member.Id, Is.EqualTo(memberId));
        Assert.That(member.ClubId, Is.EqualTo(clubId));
        Assert.That(member.Status, Is.EqualTo(Status.ACTIVE));
    }

    [Test]
    public void ApplyMemberLockEventTest()
    {
        // Given
        var member = new Member
        {
            Id = Guid.NewGuid(),
            Status = Status.ACTIVE
        };
        var memberEvents = new List<BaseEvent>
        {
            new()
            {
                EventType = EventType.MEMBER_LOCKED,
                EventData = new MemberLockedEvent()
            }
        };

        // When
        member.Apply(memberEvents);

        // Then
        Assert.That(member.Status, Is.EqualTo(Status.LOCKED));
    }

    [Test]
    public void ApplyMemberUnlockEventTest()
    {
        // Given
        var member = new Member
        {
            Id = Guid.NewGuid(),
            Status = Status.LOCKED
        };
        var memberEvents = new List<BaseEvent>
        {
            new()
            {
                EventType = EventType.MEMBER_UNLOCKED,
                EventData = new MemberUnlockedEvent()
            }
        };

        // When
        member.Apply(memberEvents);

        // Then
        Assert.That(member.Status, Is.EqualTo(Status.ACTIVE));
    }

    [Test]
    public void ApplyMemberDeleteEventTest()
    {
        // Given
        var member = new Member
        {
            Id = Guid.NewGuid(),
            Status = Status.ACTIVE
        };
        var memberEvents = new List<BaseEvent>
        {
            new()
            {
                EventType = EventType.MEMBER_DELETED,
                EventData = new MemberDeletedEvent()
            }
        };

        // When
        member.Apply(memberEvents);

        // Then
        Assert.That(member.Status, Is.EqualTo(Status.DELETED));
    }

    [Test]
    public void ApplyMemberNameChangedEventTest()
    {
        // Given
        var member = new Member
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe"
        };

        var memberEvents = new List<BaseEvent>
        {
            new()
            {
                EventType = EventType.MEMBER_FULL_NAME_CHANGED,
                EventData = new MemberFullNameChangedEvent
                {
                    FullName = new FullName { FirstName = "Jane", LastName = "Doe" }
                }
            }
        };

        // When
        member.Apply(memberEvents);

        // Then
        Assert.That(member.FirstName, Is.EqualTo("Jane"));
        Assert.That(member.LastName, Is.EqualTo("Doe"));
    }

    [Test]
    public void ApplyMemberEmailChangedEvent()
    {
        // Given
        var member = new Member
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "test@test.test"
        };

        var memberEvents = new List<BaseEvent>
        {
            new()
            {
                EventType = EventType.MEMBER_EMAIL_CHANGED,
                EventData = new MemberEmailChangedEvent
                {
                    Email = "changedEmail@com.com"
                }

            }
        };

        //When
        member.Apply(memberEvents);

        //Then
        Assert.That(member.Email, Is.EqualTo("changedEmail@com.com"));
    }
}