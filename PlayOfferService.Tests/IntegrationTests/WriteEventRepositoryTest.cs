using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.PlayOffer;

namespace PlayOfferService.Tests.IntegrationTests;

public class WriteEventRepositoryTest : TestSetup
{
    [SetUp]
    public async Task WriteEventSetup()
    {
        var existingEvent = new BaseEvent
        {
            EntityId = Guid.Parse("e77304ae-421f-445c-a0e6-bfee24e6ba60"),
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.Parse("3b84c030-5d1e-462e-8989-a8bb1407a41c"),
            EventType = EventType.PLAYOFFER_CREATED,
            Timestamp = DateTime.UtcNow,
            EventData = new PlayOfferCreatedEvent
            {
                Id = Guid.Parse("e77304ae-421f-445c-a0e6-bfee24e6ba60"),
                ClubId = Guid.Parse("b5aa9ecc-09a8-49ec-8d98-305b9a3ff772"),
                CreatorId = Guid.Parse("be7cb6a5-612f-4aa3-9f6e-2c25d812ed49"),
                ProposedStartTime = DateTime.UtcNow.AddHours(1),
                ProposedEndTime = DateTime.UtcNow.AddHours(3)
            }
        };
        
        await TestWriteEventRepository.AppendEvent(existingEvent);
        await TestWriteEventRepository.Update();
    }
    
    [Test]
    public async Task GetExistingEventByIdTest()
    {
        // Given
        var eventId = Guid.Parse("3b84c030-5d1e-462e-8989-a8bb1407a41c");

        // When
        var baseEvent = await TestWriteEventRepository.GetEventById(eventId);

        // Then
        Assert.That(baseEvent, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(baseEvent!.EntityId, Is.EqualTo(Guid.Parse("e77304ae-421f-445c-a0e6-bfee24e6ba60")));
            Assert.That(baseEvent.EntityType, Is.EqualTo(EntityType.PLAYOFFER));
            Assert.That(baseEvent.EventId, Is.EqualTo(Guid.Parse("3b84c030-5d1e-462e-8989-a8bb1407a41c")));
            Assert.That(baseEvent.EventType, Is.EqualTo(EventType.PLAYOFFER_CREATED));
            Assert.That(baseEvent.Timestamp, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
            Assert.That(baseEvent.EventData, Is.TypeOf<PlayOfferCreatedEvent>());
            var eventData = baseEvent.EventData as PlayOfferCreatedEvent;
            Assert.That(eventData!.Id, Is.EqualTo(Guid.Parse("e77304ae-421f-445c-a0e6-bfee24e6ba60")));
            Assert.That(eventData.ClubId, Is.EqualTo(Guid.Parse("b5aa9ecc-09a8-49ec-8d98-305b9a3ff772")));
            Assert.That(eventData.CreatorId, Is.EqualTo(Guid.Parse("be7cb6a5-612f-4aa3-9f6e-2c25d812ed49")));
            Assert.That(eventData.ProposedStartTime, Is.EqualTo(DateTime.UtcNow.AddHours(1)).Within(1).Seconds);
            Assert.That(eventData.ProposedEndTime, Is.EqualTo(DateTime.UtcNow.AddHours(3)).Within(1).Seconds);
        });
    }
    
    [Test]
    public async Task GetExistingEventByEntityIdTest()
    {
        // Given
        var entityId = Guid.Parse("e77304ae-421f-445c-a0e6-bfee24e6ba60");

        // When
        var baseEvents = await TestWriteEventRepository.GetEventByEntityId(entityId);

        // Then
        Assert.That(baseEvents, Has.Count.EqualTo(1));
        var baseEvent = baseEvents.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(baseEvent!.EntityId, Is.EqualTo(Guid.Parse("e77304ae-421f-445c-a0e6-bfee24e6ba60")));
            Assert.That(baseEvent.EntityType, Is.EqualTo(EntityType.PLAYOFFER));
            Assert.That(baseEvent.EventId, Is.EqualTo(Guid.Parse("3b84c030-5d1e-462e-8989-a8bb1407a41c")));
            Assert.That(baseEvent.EventType, Is.EqualTo(EventType.PLAYOFFER_CREATED));
            Assert.That(baseEvent.Timestamp, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
            Assert.That(baseEvent.EventData, Is.TypeOf<PlayOfferCreatedEvent>());
            var eventData = baseEvent.EventData as PlayOfferCreatedEvent;
            Assert.That(eventData!.Id, Is.EqualTo(Guid.Parse("e77304ae-421f-445c-a0e6-bfee24e6ba60")));
            Assert.That(eventData.ClubId, Is.EqualTo(Guid.Parse("b5aa9ecc-09a8-49ec-8d98-305b9a3ff772")));
            Assert.That(eventData.CreatorId, Is.EqualTo(Guid.Parse("be7cb6a5-612f-4aa3-9f6e-2c25d812ed49")));
            Assert.That(eventData.ProposedStartTime, Is.EqualTo(DateTime.UtcNow.AddHours(1)).Within(1).Seconds);
            Assert.That(eventData.ProposedEndTime, Is.EqualTo(DateTime.UtcNow.AddHours(3)).Within(1).Seconds);
        });
    }

    [Test]
    public async Task GetNonExistingEventByIdTest()
    {
        // Given
        var eventId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        // When
        var baseEvent = await TestWriteEventRepository.GetEventById(eventId);

        // Then
        Assert.That(baseEvent, Is.Null);
    }

    [Test]
    public async Task AppendEventTest()
    {
        // Given
        var givenEvent = new BaseEvent
        {
            EntityId = Guid.Parse("8bd27cfb-f84d-41e7-8738-901e27b97555"),
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.Parse("94dfa918-80f3-4dd7-9cb1-78ec4c71ce2e"),
            EventType = EventType.PLAYOFFER_CREATED,
            Timestamp = DateTime.UtcNow,
            EventData = new PlayOfferCreatedEvent
            {
                Id = Guid.Parse("8bd27cfb-f84d-41e7-8738-901e27b97555"),
                ClubId = Guid.Parse("09b884f1-9fbb-4d39-b3a0-a8f676bc00e3"),
                CreatorId = Guid.Parse("cd03259c-e584-46d1-b79c-3dd25ac4ed93"),
                ProposedStartTime = DateTime.UtcNow.AddHours(1),
                ProposedEndTime = DateTime.UtcNow.AddHours(3)
            }
        };

        // When
        await TestWriteEventRepository.AppendEvent(givenEvent);
        await TestWriteEventRepository.Update();

        // Then
        var baseEvent = await TestWriteEventRepository.GetEventById(givenEvent.EventId);
        Assert.That(baseEvent, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(baseEvent!.EntityId, Is.EqualTo(Guid.Parse("8bd27cfb-f84d-41e7-8738-901e27b97555")));
            Assert.That(baseEvent.EntityType, Is.EqualTo(EntityType.PLAYOFFER));
            Assert.That(baseEvent.EventId, Is.EqualTo(Guid.Parse("94dfa918-80f3-4dd7-9cb1-78ec4c71ce2e")));
            Assert.That(baseEvent.EventType, Is.EqualTo(EventType.PLAYOFFER_CREATED));
            Assert.That(baseEvent.Timestamp, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
        });
    }
}