using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Domain.Events.Reservation;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.ValueObjects;

namespace PlayOfferService.Tests.IntegrationTests;

public class ReservationEventHandlerTest : TestSetup
{
    [SetUp]
    public async Task ReservationSetup()
    {
        var existingReservation = new Reservation
        {
            Id = Guid.Parse("da9ff928-cfb2-4b98-9388-937905556706"),
            CourtIds = new List<Guid> {Guid.Parse("5b0ae85c-97fa-4e2d-900f-c5d9239f64b5")},
            ReservantId = Guid.Parse("844b93a3-fce8-4d8f-8a8d-38da07a9c11f"),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            ParticipantsIds = new List<Guid> {Guid.Parse("844b93a3-fce8-4d8f-8a8d-38da07a9c11f"), Guid.Parse("13158d40-c04f-49e8-aa0c-b2473145ceaf")},
            IsCancelled = false
        };
        TestReservationRepository.CreateReservation(existingReservation);
        await TestReservationRepository.Update();
    }
    
    [Test]
    public async Task ReservationCreatedEvent_ProjectionTest()
    {
        //Given
        var reservationCreatedEvent = new TechnicalReservationEvent
        {
            EntityId = Guid.Parse("96e78720-94e2-413e-84e4-795daefa040f"),
            EntityType = EntityType.Reservation,
            EventId = Guid.Parse("e8d1ed39-6f72-405e-b4e2-0f72bffbf128"),
            EventType = EventType.ReservationCreatedEvent,
            EventData = new ReservationCreatedEvent
            {
                CourtIds = [Guid.Parse("e23e04e1-392e-4907-a96e-9adaec407078")],
                ReservantId = Guid.Parse("0f26a0e3-a23b-425d-b54c-f6952c357198"),
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(1),
                ParticipantIds = [Guid.Parse("0f26a0e3-a23b-425d-b54c-f6952c357198")],
                TournamentId = Guid.NewGuid(),
            }
        };
        
        //When
        await Mediator.Send(reservationCreatedEvent);
        
        //When
        var projectedReservation = await TestReservationRepository.GetReservationById(reservationCreatedEvent.EntityId);
        
        //Then
        Assert.That(projectedReservation, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedReservation!.Id, Is.EqualTo(Guid.Parse("96e78720-94e2-413e-84e4-795daefa040f")));
            Assert.That(projectedReservation.CourtIds, Is.EqualTo(new List<Guid> {Guid.Parse("e23e04e1-392e-4907-a96e-9adaec407078")}));
            Assert.That(projectedReservation.ReservantId, Is.EqualTo(Guid.Parse("0f26a0e3-a23b-425d-b54c-f6952c357198")));
            Assert.That(projectedReservation.StartTime, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
            Assert.That(projectedReservation.EndTime, Is.EqualTo(DateTime.UtcNow.AddHours(1)).Within(1).Seconds);
            Assert.That(projectedReservation.ParticipantsIds, Is.EqualTo(new List<Guid> {Guid.Parse("0f26a0e3-a23b-425d-b54c-f6952c357198")}));
            Assert.That(projectedReservation.IsCancelled, Is.False);
        });
    }

    [Test]
    public async Task ReservationCancelledEvent_ProjectionTest()
    {
        // Given
        var reservationCancelledEvent = new TechnicalReservationEvent
        {
            EntityId = Guid.Parse("da9ff928-cfb2-4b98-9388-937905556706"),
            EntityType = EntityType.Reservation,
            EventId = Guid.Parse("25a7fc3e-512b-47f6-b91b-e1d2e48c10cb"),
            EventType = EventType.ReservationCancelledEvent,
            EventData = new ReservationCancelledEvent()
        };
        
        // When
        await Mediator.Send(reservationCancelledEvent);
        
        // Then
        var projectedReservation = await TestReservationRepository.GetReservationById(reservationCancelledEvent.EntityId);
        
        Assert.That(projectedReservation, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedReservation!.Id, Is.EqualTo(Guid.Parse("da9ff928-cfb2-4b98-9388-937905556706")));
            Assert.That(projectedReservation.CourtIds, Is.EqualTo(new List<Guid> {Guid.Parse("5b0ae85c-97fa-4e2d-900f-c5d9239f64b5")}));
            Assert.That(projectedReservation.ReservantId, Is.EqualTo(Guid.Parse("844b93a3-fce8-4d8f-8a8d-38da07a9c11f")));
            Assert.That(projectedReservation.StartTime, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
            Assert.That(projectedReservation.EndTime, Is.EqualTo(DateTime.UtcNow.AddHours(1)).Within(1).Seconds);
            Assert.That(projectedReservation.ParticipantsIds, Is.EqualTo(new List<Guid> {Guid.Parse("844b93a3-fce8-4d8f-8a8d-38da07a9c11f"), Guid.Parse("13158d40-c04f-49e8-aa0c-b2473145ceaf")}));
            Assert.That(projectedReservation.IsCancelled, Is.True);
        });
    }
}