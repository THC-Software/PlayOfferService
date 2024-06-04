using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.PlayOffer;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Tests.UnitTests;

[TestFixture]
public class PlayOfferUnitTest
{
    [Test]
    public void ApplyPlayOfferCreatedEventTest()
    {
        // Given
        var playOfferId = Guid.NewGuid();
        var clubId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var playOfferCreatedEvent = new BaseEvent
        {
            EventType = EventType.PLAYOFFER_CREATED,
            EventData = new PlayOfferCreatedEvent
            {
                Id = playOfferId,
                ClubId = clubId,
                CreatorId = creatorId,
                ProposedStartTime = DateTime.UtcNow.AddHours(2),
                ProposedEndTime = DateTime.UtcNow.AddHours(5)
            }
        };
        
        // When
        var playOffer = new PlayOffer();
        playOffer.Apply([playOfferCreatedEvent]);
        
        // Then
        Assert.That(playOffer.Id, Is.EqualTo(playOfferId));
        Assert.That(playOffer.ClubId, Is.EqualTo(clubId));
        Assert.That(playOffer.CreatorId, Is.EqualTo(creatorId));
        Assert.That(playOffer.IsCancelled, Is.False);
        Assert.That(playOffer.ProposedStartTime, Is.EqualTo(((PlayOfferCreatedEvent)playOfferCreatedEvent.EventData).ProposedStartTime));
        Assert.That(playOffer.ProposedEndTime, Is.EqualTo(((PlayOfferCreatedEvent)playOfferCreatedEvent.EventData).ProposedEndTime));
    }
    
    [Test]
    public void ApplyPlayOfferCancelledEvent()
    {
        // Given
        var playOfferEvent = new BaseEvent
        {
            EventType = EventType.PLAYOFFER_CANCELLED,
            EventData = new PlayOfferCancelledEvent()
        };

        // When
        var playOffer = new PlayOffer();
        playOffer.Apply([playOfferEvent]);

        // Then
        Assert.That(playOffer.IsCancelled, Is.True);
    }

    [Test]
    public void ApplyPlayOfferJoinedEvent()
    {
        // Given
        var opponentId = Guid.NewGuid();
        var acceptedStartTime = DateTime.UtcNow.AddHours(3);
        
        var playOfferEvent = new BaseEvent {
            EventType = EventType.PLAYOFFER_JOINED,
            EventData = new PlayOfferJoinedEvent
            {
                OpponentId = opponentId,
                AcceptedStartTime = acceptedStartTime
            }
        };
        
        // When
        var playOffer = new PlayOffer();
        playOffer.Apply([playOfferEvent]);
        
        // Then
        Assert.That(playOffer.OpponentId, Is.EqualTo(opponentId));
        Assert.That(playOffer.AcceptedStartTime, Is.EqualTo(acceptedStartTime));
    }
    
    [Test]
    public void ApplyPlayOfferReservationAddedEvent()
    {
        // Given
        var reservationId = Guid.NewGuid();
        var playOfferEvent = new BaseEvent 
        {
            EventType = EventType.PLAYOFFER_RESERVATION_ADDED,
            EventData = new PlayOfferReservationAddedEvent
            {
                ReservationId = reservationId
            }
        };
        
        // When
        var playOffer = new PlayOffer();
        playOffer.Apply([playOfferEvent]);
        
        // Then
        Assert.That(playOffer.ReservationId, Is.EqualTo(reservationId));
    }
    
    [Test]
    public void ApplyPlayOfferOpponentRemovedEvent()
    {
        // Given
        var givenPlayOffer = new PlayOffer
        {
            OpponentId = Guid.NewGuid()
        };
        
        var playOfferEvent = new BaseEvent
        {
            EventType = EventType.PLAYOFFER_OPPONENT_REMOVED,
            EventData = new PlayOfferOpponentRemovedEvent()
        };
        
        // When
        givenPlayOffer.Apply([playOfferEvent]);
        
        // Then
        Assert.That(givenPlayOffer.OpponentId, Is.Null);
    }
}