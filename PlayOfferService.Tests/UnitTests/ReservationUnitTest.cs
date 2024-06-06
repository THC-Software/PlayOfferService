using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Reservation;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Tests.UnitTests;

public class ReservationUnitTest
{
    [Test]
    public void ApplyReservationCreatedEvent()
    {
        // Given
        var reservationCreatedEvent = new BaseEvent
        {
            EntityId = Guid.Parse("b68ff7a7-94a0-4807-80bc-7b5a0a75264f"),
            EntityType = EntityType.Reservation,
            EventId = Guid.Parse("29cfce18-81df-4741-96b0-2ac129877d07"),
            EventType = EventType.ReservationCreatedEvent,
            EventData = new ReservationCreatedEvent
            {
                CourtIds = [Guid.Parse("b67599e8-f8d7-4956-bbfc-be70c69d7b4b")],
                ReservantId = Guid.Parse("fd834838-748a-435c-a3a4-a4e93b2d4f40"),
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(1),
                ParticipantIds = [Guid.Parse("ad51b156-fa81-4403-a8c3-69c8908f31f8")],
                TournamentId = Guid.NewGuid(),
            }
        };
        
        // When
        var reservation = new Reservation();
        reservation.Apply([reservationCreatedEvent]);
        
        // Then
        Assert.Multiple(() =>
        {
            Assert.That(reservation.Id, Is.EqualTo(Guid.Parse("b68ff7a7-94a0-4807-80bc-7b5a0a75264f")));
            Assert.That(reservation.CourtIds, Is.EqualTo(new List<Guid> {Guid.Parse("b67599e8-f8d7-4956-bbfc-be70c69d7b4b")}));
            Assert.That(reservation.ReservantId, Is.EqualTo(Guid.Parse("fd834838-748a-435c-a3a4-a4e93b2d4f40")));
            Assert.That(reservation.StartTime, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
            Assert.That(reservation.EndTime, Is.EqualTo(DateTime.UtcNow.AddHours(1)).Within(1).Seconds);
            Assert.That(reservation.ParticipantsIds, Is.EqualTo(new List<Guid> {Guid.Parse("ad51b156-fa81-4403-a8c3-69c8908f31f8")}));
            Assert.That(reservation.IsCancelled, Is.False);
        });
    }
    
    [Test]
    public void ApplyReservationCancelledEvent()
    {
        // Given
        var givenReservation = new Reservation
        {
            Id = Guid.NewGuid(),
            CourtIds = new List<Guid> {Guid.NewGuid()},
            ReservantId = Guid.NewGuid(),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            ParticipantsIds = new List<Guid> {Guid.NewGuid()},
            IsCancelled = false
        };
        var reservationCancelledEvent = new BaseEvent
        {
            EntityId = Guid.Parse("49c52e8a-bb50-46a4-9163-7da430cf64c0"),
            EntityType = EntityType.Reservation,
            EventId = Guid.Parse("534a9e3b-ebc7-40fa-ad21-cff5bc1827f4"),
            EventType = EventType.ReservationCancelledEvent,
            EventData = new ReservationCancelledEvent()
        };
        
        // When
        givenReservation.Apply([reservationCancelledEvent]);
        
        // Then
        Assert.That(givenReservation.IsCancelled, Is.True);
    }
}