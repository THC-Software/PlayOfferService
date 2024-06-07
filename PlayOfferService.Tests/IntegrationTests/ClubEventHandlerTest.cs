using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.ValueObjects;

namespace PlayOfferService.Tests.IntegrationTests;

public class ClubEventHandlerTest : TestSetup
{
    [SetUp]
    public async Task ClubSetup()
    {
        var existingClubs = new List<Club>
        {
            new()
            {
                Id = Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa"),
                Status = Status.ACTIVE
            },
            new()
            {
                Id = Guid.Parse("2db387f0-5792-4cb5-91a6-6317437b3432"),
                Status = Status.ACTIVE
            }
        };
        
        foreach (var club in existingClubs)
        {
            TestClubRepository.CreateClub(club);
        }
        await TestClubRepository.Update();
        
        var existingPlayOffers = new List<PlayOffer>
        {
            new()
            {
                Id = Guid.Parse("d9afd6dd-8836-47dd-86be-3467a7db0fe5"),
                ClubId = Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa"),
                CreatorId = Guid.Parse("4f5d3d99-1b8e-4276-8eff-32d484eef56c"),
                ProposedStartTime = DateTime.UtcNow,
                ProposedEndTime = DateTime.UtcNow.AddHours(3),
                IsCancelled = false
            },
            new()
            {
                Id = Guid.Parse("9ee6d9d7-d4a6-4904-a20b-be026de53c4f"),
                ClubId = Guid.Parse("2db387f0-5792-4cb5-91a6-6317437b3432"),
                CreatorId = Guid.Parse("c18e24c1-21bf-4505-ae15-e1960c0f7a9b"),
                ProposedStartTime = DateTime.UtcNow,
                ProposedEndTime = DateTime.UtcNow.AddHours(3),
                IsCancelled = false
            },
        };
        
        foreach (var playOffer in existingPlayOffers)
        {
            TestPlayOfferRepository.CreatePlayOffer(playOffer);
        }
        
        await TestPlayOfferRepository.Update();
    }
    
    [Test]
    public async Task ClubCreatedEvent_ProjectionTest()
    {
        //Given
        var clubId = Guid.NewGuid();
        var clubCreationEvent = new TechnicalClubEvent
        {
            EntityId = clubId,
            EntityType = EntityType.TENNIS_CLUB,
            EventId = Guid.NewGuid(),
            EventType = EventType.TENNIS_CLUB_REGISTERED,
            EventData = new ClubCreatedEvent
            {
                TennisClubId = new TennisClubId {Id = clubId}
            }
        };
        
        //When
        await Mediator.Send(clubCreationEvent);
        
        //Then
        var projectedClub = await TestClubRepository.GetClubById(clubId);
        
        Assert.That(projectedClub, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedClub!.Id, Is.EqualTo(clubId));
            Assert.That(projectedClub.Status, Is.EqualTo(Status.ACTIVE));
        });
    }
    
    [Test]
    public async Task ClubLockedEvent_ProjectionTest()
    {
        //Given
        var clubLockedEvent = new TechnicalClubEvent
        {
            EntityId = Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa"),
            EntityType = EntityType.TENNIS_CLUB,
            EventId = Guid.Parse("ee90c3ee-3b4b-4ccb-8933-739795a7253a"),
            EventType = EventType.TENNIS_CLUB_LOCKED,
            EventData = new ClubLockedEvent()
        };
        
        //When
        await Mediator.Send(clubLockedEvent);
        
        //Then
        var projectedClub = await TestClubRepository.GetClubById(Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa"));
        
        Assert.That(projectedClub, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedClub!.Id, Is.EqualTo(Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa")));
            Assert.That(projectedClub.Status, Is.EqualTo(Status.LOCKED));
        });
        
        var playOfferEvents = await TestWriteEventRepository.GetEventByEntityId(Guid.Parse("d9afd6dd-8836-47dd-86be-3467a7db0fe5"));
        Assert.That(playOfferEvents, Has.Count.EqualTo(1));
        
        Assert.Multiple(() =>
        {
            Assert.That(playOfferEvents.First().EntityId, Is.EqualTo(Guid.Parse("d9afd6dd-8836-47dd-86be-3467a7db0fe5")));
            Assert.That(playOfferEvents.First().EventType, Is.EqualTo(EventType.PLAYOFFER_CANCELLED));
            Assert.That(playOfferEvents.First().CorrelationId, Is.EqualTo(Guid.Parse("ee90c3ee-3b4b-4ccb-8933-739795a7253a")));
        });
    }
    
    [Test]
    public async Task ClubUnlockedEvent_ProjectionTest()
    {
        //Given
        var clubUnlockedEvent = new TechnicalClubEvent
        {
            EntityId = Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa"),
            EntityType = EntityType.TENNIS_CLUB,
            EventId = Guid.NewGuid(),
            EventType = EventType.TENNIS_CLUB_UNLOCKED,
            EventData = new ClubUnlockedEvent()
        };

        //When
        await Mediator.Send(clubUnlockedEvent);

        //Then
        var projectedClub = await TestClubRepository.GetClubById(Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa"));

        Assert.That(projectedClub, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedClub!.Id, Is.EqualTo(Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa")));
            Assert.That(projectedClub.Status, Is.EqualTo(Status.ACTIVE));
        });
    }
    
    [Test]
    public async Task ClubDeletedEvent_ProjectionTest()
    {
        //Given
        var clubDeletedEvent = new TechnicalClubEvent
        {
            EntityId = Guid.Parse("2db387f0-5792-4cb5-91a6-6317437b3432"),
            EntityType = EntityType.TENNIS_CLUB,
            EventId = Guid.Parse("ab0ecc19-e492-4126-aac5-5448198d62cc"),
            EventType = EventType.TENNIS_CLUB_DELETED,
            EventData = new ClubDeletedEvent()
        };

        //When
        await Mediator.Send(clubDeletedEvent);

        //Then
        var projectedClub = await TestClubRepository.GetClubById(Guid.Parse("2db387f0-5792-4cb5-91a6-6317437b3432"));

        Assert.That(projectedClub, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedClub!.Id, Is.EqualTo(Guid.Parse("2db387f0-5792-4cb5-91a6-6317437b3432")));
            Assert.That(projectedClub.Status, Is.EqualTo(Status.DELETED));
        });
        
        var playOfferEvents = await TestWriteEventRepository.GetEventByEntityId(Guid.Parse("9ee6d9d7-d4a6-4904-a20b-be026de53c4f"));
        Assert.That(playOfferEvents, Has.Count.EqualTo(1));
        
        Assert.Multiple(() =>
        {
            Assert.That(playOfferEvents.First().EntityId, Is.EqualTo(Guid.Parse("9ee6d9d7-d4a6-4904-a20b-be026de53c4f")));
            Assert.That(playOfferEvents.First().EventType, Is.EqualTo(EventType.PLAYOFFER_CANCELLED));
            Assert.That(playOfferEvents.First().CorrelationId, Is.EqualTo(Guid.Parse("ab0ecc19-e492-4126-aac5-5448198d62cc")));
        });
    }
}