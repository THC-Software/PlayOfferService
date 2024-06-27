using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Court;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Tests.UnitTests;

public class CourtUnitTest
{
    [Test]
    public void ApplyCourtCreatedEventTest()
    {
        // Given
        var courtCreatedEvent = new BaseEvent {
            EventType = EventType.CourtCreatedEvent,
            EntityId = Guid.Parse("d2c0c38c-2680-41ad-a50a-6e9a18408c8b"),
            EventData = new CourtCreatedEvent {
                ClubId = Guid.Parse("353b10b4-97d6-4eff-aef3-7a4a37db91ff"),
                Name = "TestCourt 1"
            }
        };
        
        // When
        var court = new Court();
        court.Apply([courtCreatedEvent]);
        
        // Then
        Assert.Multiple(() => {
            Assert.That(court.Id, Is.EqualTo(Guid.Parse("d2c0c38c-2680-41ad-a50a-6e9a18408c8b")));
            Assert.That(court.ClubId, Is.EqualTo(Guid.Parse("353b10b4-97d6-4eff-aef3-7a4a37db91ff")));
            Assert.That(court.Name, Is.EqualTo("TestCourt 1"));
        });
    }
    
    public void ApplyCourtUpdatedEventTest()
    {
        // Given
        var courtUpdatedEvent = new BaseEvent {
            EventType = EventType.CourtUpdatedEvent,
            EntityId = Guid.Parse("bb50fb5c-534b-47df-ac91-29eec0447e9b"),
            EventData = new CourtUpdatedEvent {
                CourtName = "TestCourt 1 - Updated"
            }
        };
        var givenCourt = new Court {
            Id = Guid.Parse("bb50fb5c-534b-47df-ac91-29eec0447e9b"),
            ClubId = Guid.Parse("724ecde4-7da9-480f-b84d-95ba4346ade9"),
            Name = "TestCourt 1"
        };
        
        // When
        givenCourt.Apply([courtUpdatedEvent]);
        
        // Then
        Assert.Multiple(() => {
            Assert.That(givenCourt.Id, Is.EqualTo(Guid.Parse("bb50fb5c-534b-47df-ac91-29eec0447e9b")));
            Assert.That(givenCourt.ClubId, Is.EqualTo(Guid.Parse("724ecde4-7da9-480f-b84d-95ba4346ade9")));
            Assert.That(givenCourt.Name, Is.EqualTo("TestCourt 1 - Updated"));
        });
    }
}