using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.ValueObjects;

namespace PlayOfferService.Tests.IntegrationTests;

public class ClubEventHandlerTest : TestSetup
{
    [SetUp]
    public async Task ClubSetup()
    {
        var existingClub = new Club
        {
            Id = Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa"),
            Name = "Existing Club",
            Status = Status.ACTIVE
        };
        TestClubRepository.CreateClub(existingClub);
        await TestClubRepository.Update();
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
                TennisClubId = new TennisClubId { Id = clubId },
                Name = "Test Club",
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
            EventId = Guid.NewGuid(),
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
            EntityId = Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa"),
            EntityType = EntityType.TENNIS_CLUB,
            EventId = Guid.NewGuid(),
            EventType = EventType.TENNIS_CLUB_DELETED,
            EventData = new ClubDeletedEvent()
        };

        //When
        await Mediator.Send(clubDeletedEvent);

        //Then
        var projectedClub = await TestClubRepository.GetClubById(Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa"));

        Assert.That(projectedClub, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedClub!.Id, Is.EqualTo(Guid.Parse("8aa54411-32fe-4b4c-a017-aa9710cb3bfa")));
            Assert.That(projectedClub.Status, Is.EqualTo(Status.DELETED));
        });
    }
}