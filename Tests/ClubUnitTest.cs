using NUnit.Framework;
using PlayOfferService.Domain.Events;
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
        var clubCreationEvent = new BaseEvent<IDomainEvent>
        {
            EntityId = clubId,
            EntityType = EntityType.CLUB,
            EventId = Guid.NewGuid(),
            EventType = EventType.TENNIS_CLUB_REGISTERED,
            EventData = new ClubCreatedEvent
            {
                TennisClubId = clubId
            }
        };
        
        // When
        var club = new Club();
        club.Apply([clubCreationEvent]);
        
        // Then
        Assert.That(club.Id, Is.EqualTo(clubId));
        Assert.That(club.IsLocked, Is.False);
    }
    
    [Test]
    public void ApplyClubLockEventTest()
    {
        // Given
        var clubEvents = new List<BaseEvent<IDomainEvent>>
        {
            new()
            {
                EventType = EventType.TENNIS_CLUB_REGISTERED,
                EventData = new ClubCreatedEvent()
            },
            new()
            {
                EventType = EventType.TENNIS_CLUB_LOCKED,
                EventData = new ClubLockedEvent()
            }
        };
        
        // When
        var club = new Club();
        club.Apply(clubEvents);
        
        // Then
        Assert.That(club.IsLocked, Is.True);
    }
    
    [Test]
    public void ApplyClubUnlockEventTest()
    {
        // Given
        var clubEvents = new List<BaseEvent<IDomainEvent>>
        {
            new()
            {
                EventType = EventType.TENNIS_CLUB_REGISTERED,
                EventData = new ClubCreatedEvent()
            },
            new()
            {
                EventType = EventType.TENNIS_CLUB_LOCKED,
                EventData = new ClubLockedEvent()
            },
            new()
            {
                EventType = EventType.TENNIS_CLUB_UNLOCKED,
                EventData = new ClubUnlockedEvent()
            }
        };
        
        // When
        var club = new Club();
        club.Apply(clubEvents);
        
        // Then
        Assert.That(club.IsLocked, Is.False);
    }
}