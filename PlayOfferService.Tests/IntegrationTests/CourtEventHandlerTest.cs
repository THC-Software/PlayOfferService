using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Court;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Tests.IntegrationTests;

public class CourtEventHandlerTest : TestSetup
{
    [SetUp]
    public async Task CourtSetup()
    {
        var existingCourt = new Court {
            Id = Guid.Parse("3e47acd8-55d6-4042-8fc2-69c3ee9fae31"),
            ClubId = Guid.Parse("588cf6e7-04a8-4de4-99d0-5ec2b0daa19a"),
            Name = "TestCourt 10"
        };
        TestCourtRepository.CreateCourt(existingCourt);
        await TestReservationRepository.Update();
    }
    
    [Test]
    public async Task CourtCreatedEvent_ProjectionTest()
    {
        // Given
        var courtCreatedEvent = new TechnicalCourtEvent {
            EventId = Guid.Parse("fac8ccd9-b523-4cff-bcb6-db63bdf9207f"),
            EventType = EventType.CourtCreatedEvent,
            EntityId = Guid.Parse("d2c39afd-5e1d-44c0-b339-c0b53e8eee2d"),
            EventData = new CourtCreatedEvent {
                ClubId = Guid.Parse("6e20d048-836b-48d0-8031-36a40353bc6a"),
                Name = "TestCourt 20"
            }
        };
        
        // When
        await Mediator.Send(courtCreatedEvent);
        
        // Then
        var projectedCourt = await TestCourtRepository.GetCourtById(courtCreatedEvent.EntityId);
        
        Assert.That(projectedCourt, Is.Not.Null);
        Assert.Multiple(() => {
            Assert.That(projectedCourt!.Id, Is.EqualTo(Guid.Parse("d2c39afd-5e1d-44c0-b339-c0b53e8eee2d")));
            Assert.That(projectedCourt.ClubId, Is.EqualTo(Guid.Parse("6e20d048-836b-48d0-8031-36a40353bc6a")));
            Assert.That(projectedCourt.Name, Is.EqualTo("TestCourt 20"));
        });
    }
    
    [Test]
    public async Task CourtUpdatedEvent_ProjectionTest()
    {
        // Given
        var courtUpdatedEvent = new TechnicalCourtEvent {
            EventId = Guid.Parse("2cc26a0f-b75c-4d1f-8098-8dcb02cb3685"),
            EventType = EventType.CourtUpdatedEvent,
            EntityId = Guid.Parse("3e47acd8-55d6-4042-8fc2-69c3ee9fae31"),
            EventData = new CourtUpdatedEvent {
                CourtName = "TestCourt 20 - Updated"
            }
        };
        
        // When
        await Mediator.Send(courtUpdatedEvent);
        
        // Then
        var projectedCourt = await TestCourtRepository.GetCourtById(courtUpdatedEvent.EntityId);
        
        Assert.That(projectedCourt, Is.Not.Null);
        Assert.Multiple(() => {
            Assert.That(projectedCourt!.Id, Is.EqualTo(Guid.Parse("3e47acd8-55d6-4042-8fc2-69c3ee9fae31")));
            Assert.That(projectedCourt.Name, Is.EqualTo("TestCourt 20 - Updated"));
        });
    }
}