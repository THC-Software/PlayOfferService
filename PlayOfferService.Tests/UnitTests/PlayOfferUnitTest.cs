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
        var playOfferCreatedEvent = new BaseEvent
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
        var club = new Club
        {
            Id = Guid.NewGuid()
        };
        
        var playOfferEvents = new List<BaseEvent>
        {
            new()
            {
                EventType = EventType.PLAYOFFER_CREATED,
                EventData = new PlayOfferCreatedEvent
                {
                    Creator = new Member
                    {
                        Id = Guid.NewGuid()
                    },
                    Club = club,
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
                        Id = opponentId,
                        ClubId = club.Id
                    },
                    AcceptedStartTime = acceptedStartTime
                }
            }
        };
        
        // When
        var playOffer = new PlayOffer();
        playOffer.Apply(playOfferEvents);
        
        // Then
        Assert.That(playOffer.Opponent?.Id, Is.EqualTo(opponentId));
        Assert.That(playOffer.AcceptedStartTime, Is.EqualTo(acceptedStartTime));
    }
}