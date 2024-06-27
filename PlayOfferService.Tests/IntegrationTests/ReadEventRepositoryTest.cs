using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.ValueObjects;

namespace PlayOfferService.Tests.IntegrationTests;

public class ReadEventRepositoryTest : TestSetup
{
    [SetUp]
    public async Task ReadEventSetup()
    {
        var existingEvent = new BaseEvent
        {
            EntityId = Guid.Parse("eca14668-95c5-4e40-8f24-7afc155b39a1"),
            EntityType = EntityType.MEMBER,
            EventId = Guid.Parse("1609fbee-e9e2-4485-a83b-1db7ba78f384"),
            EventType = EventType.MEMBER_REGISTERED,
            Timestamp = DateTime.UtcNow,
            EventData = new MemberCreatedEvent
            {
                Email = "testemail@gmail.com",
                MemberId = new MemberId { Id = Guid.Parse("3ba41c4b-2b0e-4263-b847-518869978e4d") },
                Name = new FullName { FirstName = "Max", LastName = "Mustermann" },
                Status = Status.ACTIVE,
                TennisClubId = new TennisClubId { Id = Guid.Parse("9af8b30c-865c-4aa2-9590-9a81ab66d7a1") }
            }
        };
        await TestReadEventRepository.AppendEvent(existingEvent);
        await TestReadEventRepository.Update();
    }

    [Test]
    public async Task GetExistingEventByIdTest()
    {
        // Given
        var eventId = Guid.Parse("1609fbee-e9e2-4485-a83b-1db7ba78f384");

        // When
        var baseEvent = await TestReadEventRepository.GetEventById(eventId);

        // Then
        Assert.That(baseEvent, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(baseEvent!.EntityId, Is.EqualTo(Guid.Parse("eca14668-95c5-4e40-8f24-7afc155b39a1")));
            Assert.That(baseEvent.EntityType, Is.EqualTo(EntityType.MEMBER));
            Assert.That(baseEvent.EventId, Is.EqualTo(Guid.Parse("1609fbee-e9e2-4485-a83b-1db7ba78f384")));
            Assert.That(baseEvent.EventType, Is.EqualTo(EventType.MEMBER_REGISTERED));
            Assert.That(baseEvent.Timestamp, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
            Assert.That(baseEvent.EventData, Is.TypeOf<MemberCreatedEvent>());
            var eventData = baseEvent.EventData as MemberCreatedEvent;
            Assert.That(eventData!.Email, Is.EqualTo("testemail@gmail.com"));
            Assert.That(eventData.MemberId.Id, Is.EqualTo(Guid.Parse("3ba41c4b-2b0e-4263-b847-518869978e4d")));
            Assert.That(eventData.Name.FirstName, Is.EqualTo("Max"));
            Assert.That(eventData.Name.LastName, Is.EqualTo("Mustermann"));
            Assert.That(eventData.Status, Is.EqualTo(Status.ACTIVE));
            Assert.That(eventData.TennisClubId.Id, Is.EqualTo(Guid.Parse("9af8b30c-865c-4aa2-9590-9a81ab66d7a1")));
        });
    }

    [Test]
    public async Task GetNonExistingEventByIdTest()
    {
        // Given
        var eventId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        // When
        var baseEvent = await TestReadEventRepository.GetEventById(eventId);

        // Then
        Assert.That(baseEvent, Is.Null);
    }

    [Test]
    public async Task AppendEventTest()
    {
        // Given
        var givenEvent = new BaseEvent
        {
            EntityId = Guid.Parse("481241b5-7435-4019-bb6d-e5bf3af4a376"),
            EntityType = EntityType.MEMBER,
            EventId = Guid.Parse("73b9f922-ea88-4764-9f0f-7cf132336866"),
            EventType = EventType.MEMBER_REGISTERED,
            Timestamp = DateTime.UtcNow,
            EventData = new MemberCreatedEvent
            {
                Email = "testemail@gmail.com",
                MemberId = new MemberId { Id = Guid.Parse("93b12459-c749-4eda-8c3d-5196946e4a25") },
                Name = new FullName { FirstName = "Max", LastName = "Mustermann" },
                Status = Status.ACTIVE,
                TennisClubId = new TennisClubId { Id = Guid.Parse("148c8967-e1fb-4e43-a4a9-8a4e33e2c2ce") }
            }
        };

        // When
        await TestReadEventRepository.AppendEvent(givenEvent);
        await TestReadEventRepository.Update();

        // Then
        var baseEvent = await TestReadEventRepository.GetEventById(givenEvent.EventId);
        Assert.That(baseEvent, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(baseEvent!.EntityId, Is.EqualTo(Guid.Parse("481241b5-7435-4019-bb6d-e5bf3af4a376")));
            Assert.That(baseEvent.EntityType, Is.EqualTo(EntityType.MEMBER));
            Assert.That(baseEvent.EventId, Is.EqualTo(Guid.Parse("73b9f922-ea88-4764-9f0f-7cf132336866")));
            Assert.That(baseEvent.EventType, Is.EqualTo(EventType.MEMBER_REGISTERED));
            Assert.That(baseEvent.Timestamp, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
        });
    }
}