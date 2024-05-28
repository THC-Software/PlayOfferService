using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Models;

namespace PlayOfferService.Tests;

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
                TennisClubId = new TennisClubId{Id=clubId}
            }
        };

        // When
        var club = new Club();
        club.Apply([clubCreationEvent]);

        // and Then
        Assert.That(club.Id, Is.EqualTo(clubId));
        Assert.That(club.IsLocked, Is.False);
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
        Assert.That(club.IsLocked, Is.True);
    }

    [Test]
    public void ApplyClubUnlockEventTest()
    {
        // Given
        var club = new Club
        {
            Id = Guid.NewGuid(),
            IsLocked = true
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
        Assert.That(club.IsLocked, Is.False);
    }
}