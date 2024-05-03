using NUnit.Framework;
using PlayOfferService.Domain.Events;
using PlayOfferService.Models;

namespace PlayOfferService.Tests;

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
        var playOfferCreatedEvent = new BaseEvent<IDomainEvent>
        {
            EventType = EventType.PLAYOFFER_CREATED,
            EventData = new PlayOfferCreatedEvent
            {
                Id = playOfferId,
                Club = new Club
                {
                    Id = clubId
                },
                Creator = new Member
                {
                    Id = creatorId
                },
                ProposedStartTime = DateTime.UtcNow.AddHours(2),
                ProposedEndTime = DateTime.UtcNow.AddHours(5)
            }
        };
        
        // When
        var playOffer = new PlayOffer();
        playOffer.Apply([playOfferCreatedEvent]);
        
        // Then
        Assert.That(playOffer.Id, Is.EqualTo(playOfferId));
        Assert.That(playOffer.Club.Id, Is.EqualTo(clubId));
        Assert.That(playOffer.Creator.Id, Is.EqualTo(creatorId));
        Assert.That(playOffer.IsCancelled, Is.False);
        Assert.That(playOffer.ProposedStartTime, Is.EqualTo(((PlayOfferCreatedEvent)playOfferCreatedEvent.EventData).ProposedStartTime));
        Assert.That(playOffer.ProposedEndTime, Is.EqualTo(((PlayOfferCreatedEvent)playOfferCreatedEvent.EventData).ProposedEndTime));
    }
    
    [Test]
    public void ApplyPlayOfferCancelledEvent()
    {
        // Given
        var playOfferEvents = new List<BaseEvent<IDomainEvent>>
        {
            new()
            {
                EventType = EventType.PLAYOFFER_CREATED,
                EventData = new PlayOfferCreatedEvent
                {
                    ProposedStartTime = DateTime.UtcNow.AddHours(2),
                    ProposedEndTime = DateTime.UtcNow.AddHours(5)
                }
            },
            new()
            {
                EventType = EventType.PLAYOFFER_CANCELLED,
                EventData = new PlayOfferCancelledEvent()
            }
        };

        // When
        var playOffer = new PlayOffer();
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
        
        var playOfferEvents = new List<BaseEvent<IDomainEvent>>
        {
            new()
            {
                EventType = EventType.PLAYOFFER_CREATED,
                EventData = new PlayOfferCreatedEvent
                {
                    ProposedStartTime = DateTime.UtcNow.AddHours(2),
                    ProposedEndTime = DateTime.UtcNow.AddHours(5)
                }
            },
            new()
            {
                EventType = EventType.PLAYOFFER_JOINED,
                EventData = new PlayOfferJoinedEvent
                {
                    Opponent = new Member
                    {
                        Id = opponentId
                    },
                    AcceptedStartTime = acceptedStartTime
                }
            }
        };
        
        // When
        var playOffer = new PlayOffer();
        playOffer.Apply(playOfferEvents);
        
        // Then
        Assert.That(playOffer.Opponent.Id, Is.EqualTo(opponentId));
        Assert.That(playOffer.AcceptedStartTime, Is.EqualTo(acceptedStartTime));
    }
    
    [Test]
    public void ApplyPlayOfferJoinedEvent_InvalidAcceptedStartTime()
    {
        // Given
        var acceptedStartTime = DateTime.UtcNow;

        var playOfferEvents = new List<BaseEvent<IDomainEvent>>
        {
            new()
            {
                EventType = EventType.PLAYOFFER_CREATED,
                EventData = new PlayOfferCreatedEvent
                {
                    ProposedStartTime = DateTime.UtcNow.AddHours(2),
                    ProposedEndTime = DateTime.UtcNow.AddHours(5)
                }
            },
            new()
            {
                EventType = EventType.PLAYOFFER_JOINED,
                EventData = new PlayOfferJoinedEvent
                {
                    AcceptedStartTime = acceptedStartTime
                }
            }
        };
        
        // When & Then
        var playOffer = new PlayOffer();
        Assert.Throws<ArgumentException>(() => playOffer.Apply(playOfferEvents));
    }
    
    [Test]
    public void ApplyPlayOfferJoinedEvent_Cancelled()
    {
        // Given
        var playOfferEvents = new List<BaseEvent<IDomainEvent>>
        {
            new()
            {
                EventType = EventType.PLAYOFFER_CREATED,
                EventData = new PlayOfferCreatedEvent
                {
                    ProposedStartTime = DateTime.UtcNow.AddHours(2),
                    ProposedEndTime = DateTime.UtcNow.AddHours(5)
                }
            },
            new()
            {
                EventType = EventType.PLAYOFFER_CANCELLED,
                EventData = new PlayOfferCancelledEvent()
            },
            new()
            {
                EventType = EventType.PLAYOFFER_JOINED,
                EventData = new PlayOfferJoinedEvent()
            }
        };
        
        // When & Then
        var playOffer = new PlayOffer();
        Assert.Throws<ArgumentException>(() => playOffer.Apply(playOfferEvents));
    }
}