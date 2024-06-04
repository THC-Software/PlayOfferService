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
        var playOffer = new PlayOffer
        {
            Id = Guid.NewGuid()
        };
        var playOfferEvents = new List<BaseEvent>
        {
            new()
            {
                EventType = EventType.PLAYOFFER_CANCELLED,
                EventData = new PlayOfferCancelledEvent()
            }
        };

        // When
        playOffer.Apply(playOfferEvents);

        // Then
        Assert.That(playOffer.IsCancelled, Is.True);
    }

    [Test]
    public void ApplyPlayOfferJoinedEvent()
    {
        // Given
        var opponentId = Guid.NewGuid();
        var acceptedStartTime = DateTime.UtcNow.AddHours(3);
        var clubId = Guid.NewGuid();
        
        var playOfferEvents = new List<BaseEvent>
        {
            new()
            {
                EventType = EventType.PLAYOFFER_CREATED,
                EventData = new PlayOfferCreatedEvent
                {
                    CreatorId = Guid.NewGuid(),
                    ClubId = clubId,
                    ProposedStartTime = DateTime.UtcNow.AddHours(2),
                    ProposedEndTime = DateTime.UtcNow.AddHours(5)
                }
            },
            new()
            {
                EventType = EventType.PLAYOFFER_JOINED,
                EventData = new PlayOfferJoinedEvent
                {
                    OpponentId = opponentId,
                    AcceptedStartTime = acceptedStartTime
                }
            }
        };
        
        // When
        var playOffer = new PlayOffer();
        playOffer.Apply(playOfferEvents);
        
        // Then
        Assert.That(playOffer.OpponentId, Is.EqualTo(opponentId));
        Assert.That(playOffer.AcceptedStartTime, Is.EqualTo(acceptedStartTime));
    }
}