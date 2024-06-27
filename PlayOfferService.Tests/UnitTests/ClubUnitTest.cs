using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Club;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.ValueObjects;

namespace PlayOfferService.Tests.UnitTests;

[TestFixture]
public class ClubUnitTest
{
    [Test]
    public void ApplyClubCreatedEventTest()
    {
        // Given
        var clubId = Guid.NewGuid();
        var clubCreationEvent = new BaseEvent
        {
            EntityId = clubId,
            EntityType = EntityType.TENNIS_CLUB,
            EventId = Guid.NewGuid(),
            EventType = EventType.TENNIS_CLUB_REGISTERED,
            EventData = new ClubCreatedEvent
            {
                TennisClubId = new TennisClubId { Id = clubId }
            }
        };

        // When
        var club = new Club();
        club.Apply([clubCreationEvent]);

        // and Then
        Assert.That(club.Id, Is.EqualTo(clubId));
        Assert.That(club.Status, Is.EqualTo(Status.ACTIVE));
    }

    [Test]
    public void ApplyClubLockEventTest()
    {
        var club = new Club
        {
            Id = Guid.NewGuid()
        };
        // Given
        var clubEvents = new List<BaseEvent>
        {
            new()
            {
                EventType = EventType.TENNIS_CLUB_LOCKED,
                EventData = new ClubLockedEvent()
            }
        };

        // When
        club.Apply(clubEvents);

        // Then
        Assert.That(club.Status, Is.EqualTo(Status.LOCKED));
    }

    [Test]
    public void ApplyClubUnlockEventTest()
    {
        // Given
        var club = new Club
        {
            Id = Guid.NewGuid(),
            Status = Status.LOCKED
        };
        var clubEvents = new List<BaseEvent>
        {
            new()
            {
                EventType = EventType.TENNIS_CLUB_UNLOCKED,
                EventData = new ClubUnlockedEvent()
            }
        };

        // When
        club.Apply(clubEvents);

        // Then
        Assert.That(club.Status, Is.EqualTo(Status.ACTIVE));
    }

    [Test]
    public void ApplyClubDeletedEventTest()
    {
        var club = new Club
        {
            Id = Guid.NewGuid()
        };
        // Given
        var clubEvents = new List<BaseEvent>
        {
            new()
            {
                EventType = EventType.TENNIS_CLUB_DELETED,
                EventData = new ClubDeletedEvent()
            }
        };

        // When
        club.Apply(clubEvents);

        // Then
        Assert.That(club.Status, Is.EqualTo(Status.DELETED));
    }

    [Test]
    public void ApplyClubNameChangedEventTest()
    {
        // Given
        var club = new Club
        {
            Id = Guid.NewGuid(),
            Name = "Test Club"
        };
        var clubEvents = new List<BaseEvent>
        {
            new()
            {
                EventType = EventType.TENNIS_CLUB_NAME_CHANGED,
                EventData = new ClubNameChangedEvent
                {
                    Name = "New Club Name"
                }
            }
        };

        // When
        club.Apply(clubEvents);

        // Then
        Assert.That(club.Name, Is.EqualTo("New Club Name"));
    }

}