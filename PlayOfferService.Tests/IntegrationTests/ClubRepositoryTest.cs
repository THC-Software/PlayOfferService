using PlayOfferService.Domain.Events;
using PlayOfferService.Repositories;

namespace PlayOfferService.Tests.IntegrationTests;

[TestFixture]
public class ClubRepositoryTest : TestSetup
{
    [SetUp]
    public async Task ClubSetup()
    {
        var clubCreationEvent = new BaseEvent
        {
            EntityId = Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa"),
            EntityType = EntityType.CLUB,
            EventId = Guid.NewGuid(),
            EventType = EventType.TENNIS_CLUB_REGISTERED,
            EventData = new ClubCreatedEvent
            {
                TennisClubId = Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa")
            }
        };
        
        var lockedClubCreationEvent = new BaseEvent
        {
            EntityId = Guid.Parse("9ad0861f-89d0-40f2-899c-58525d381aac"),
            EntityType = EntityType.CLUB,
            EventId = Guid.NewGuid(),
            EventType = EventType.TENNIS_CLUB_REGISTERED,
            EventData = new ClubCreatedEvent
            {
                TennisClubId = Guid.Parse("9ad0861f-89d0-40f2-899c-58525d381aac")
            }
        };
        var clubLockedEvent = new BaseEvent
        {
            EntityId = Guid.Parse("9ad0861f-89d0-40f2-899c-58525d381aac"),
            EntityType = EntityType.CLUB,
            EventId = Guid.NewGuid(),
            EventType = EventType.TENNIS_CLUB_LOCKED,
            EventData = new ClubLockedEvent()
        };
        await TestClubRepository.UpdateEntityAsync(clubCreationEvent);
        await TestClubRepository.UpdateEntityAsync(lockedClubCreationEvent);
        await TestClubRepository.UpdateEntityAsync(clubLockedEvent);
    }
    
    [Test]
    public async Task ClubCreatedEvent_ProjectionTest()
    {
        //Given
        var clubId = Guid.NewGuid();
        var clubCreationEvent = new BaseEvent
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
        
        //When
        await TestClubRepository.UpdateEntityAsync(clubCreationEvent);
        
        //Then
        var projectedClub = await TestClubRepository.GetClubById(clubId);
        
        Assert.That(projectedClub, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedClub.Id, Is.EqualTo(clubId));
            Assert.That(projectedClub.IsLocked, Is.False);
        });
    }
    
    [Test]
    public async Task ClubLockedEvent_ProjectionTest()
    {
        //Given
        var clubLockedEvent = new BaseEvent
        {
            EntityId = Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa"),
            EntityType = EntityType.CLUB,
            EventId = Guid.NewGuid(),
            EventType = EventType.TENNIS_CLUB_LOCKED,
            EventData = new ClubLockedEvent()
        };
        
        //When
        await TestClubRepository.UpdateEntityAsync(clubLockedEvent);
        
        //Then
        var projectedClub = await TestClubRepository.GetClubById(Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa"));
        
        Assert.That(projectedClub, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedClub.Id, Is.EqualTo(Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa")));
            Assert.That(projectedClub.IsLocked, Is.True);
        });
    }

    [Test]
    public async Task ClubUnlockedEvent_ProjectionTest()
    {
        //Given
        var clubUnlockedEvent = new BaseEvent
        {
            EntityId = Guid.Parse("9ad0861f-89d0-40f2-899c-58525d381aac"),
            EntityType = EntityType.CLUB,
            EventId = Guid.NewGuid(),
            EventType = EventType.TENNIS_CLUB_UNLOCKED,
            EventData = new ClubUnlockedEvent()
        };

        //When
        await TestClubRepository.UpdateEntityAsync(clubUnlockedEvent);

        //Then
        var projectedClub = await TestClubRepository.GetClubById(Guid.Parse("9ad0861f-89d0-40f2-899c-58525d381aac"));

        Assert.That(projectedClub, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedClub.Id, Is.EqualTo(Guid.Parse("9ad0861f-89d0-40f2-899c-58525d381aac")));
            Assert.That(projectedClub.IsLocked, Is.False);
        });
    }
}