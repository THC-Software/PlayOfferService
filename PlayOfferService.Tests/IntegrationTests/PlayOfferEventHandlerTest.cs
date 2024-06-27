using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.PlayOffer;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Tests.IntegrationTests;

public class PlayOfferEventHandlerTest : TestSetup
{
    [SetUp]
    public async Task PlayOfferSetup()
    {
        var existingPlayOffers = new List<PlayOffer>{
            new()
            {
                Id = Guid.Parse("f515dc74-46ad-4a7b-b4f2-8dfda4b7506f"),
                ClubId = Guid.Parse("6031061f-86c6-459b-9260-5b4772bae9d3"),
                CreatorId = Guid.Parse("dd365b58-fa17-4df5-8d0c-a67f762f70f3"),
                ProposedStartTime = DateTime.UtcNow,
                ProposedEndTime = DateTime.UtcNow.AddHours(3),
                IsCancelled = false
            },
            new()
            {
                Id = Guid.Parse("fd385a56-17b1-4318-bc46-cf7f75f20283"),
                ClubId = Guid.Parse("6031061f-86c6-459b-9260-5b4772bae9d3"),
                CreatorId = Guid.Parse("dd365b58-fa17-4df5-8d0c-a67f762f70f3"),
                ProposedStartTime = DateTime.UtcNow,
                ProposedEndTime = DateTime.UtcNow.AddHours(3),
                IsCancelled = false,
                OpponentId = Guid.Parse("c4d9309e-a28a-4065-a213-69285ed7da5c"),
                AcceptedStartTime = DateTime.UtcNow.AddHours(1)
            },
        };
        
        foreach (var playOffer in existingPlayOffers)
        {
            TestPlayOfferRepository.CreatePlayOffer(playOffer);
        }
        await TestPlayOfferRepository.Update();
    }
    
    [Test]
    public async Task PlayOfferCreatedEvent_ProjectionTest()
    {
        //Given
        var playOfferId = Guid.NewGuid();
        var givenStartTime = DateTime.UtcNow;
        var givenEndTime = DateTime.UtcNow.AddHours(3);
        var playOfferEvent = new TechnicalPlayOfferEvent
        {
            EntityId = playOfferId,
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.NewGuid(),
            EventType = EventType.PLAYOFFER_CREATED,
            EventData = new PlayOfferCreatedEvent
            {
                Id = playOfferId,
                ClubId = Guid.Parse("437555ba-cc03-4adf-b8d8-809daccc1006"),
                CreatorId = Guid.Parse("f2d11d00-305e-492a-adc0-0e8e63328d66"),
                ProposedStartTime = givenStartTime,
                ProposedEndTime = givenEndTime
            }
        };
        
        //When
        await Mediator.Send(playOfferEvent);
        
        //Then
        var projectedPlayOffers = (await TestPlayOfferRepository.GetPlayOffersByIds(playOfferId)).ToList();
        
        Assert.That(projectedPlayOffers, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(projectedPlayOffers[0].Id, Is.EqualTo(playOfferId));
            Assert.That(projectedPlayOffers[0].ClubId, Is.EqualTo(Guid.Parse("437555ba-cc03-4adf-b8d8-809daccc1006")));
            Assert.That(projectedPlayOffers[0].CreatorId, Is.EqualTo(Guid.Parse("f2d11d00-305e-492a-adc0-0e8e63328d66")));
            Assert.That(projectedPlayOffers[0].ProposedStartTime, Is.EqualTo(givenStartTime));
            Assert.That(projectedPlayOffers[0].ProposedEndTime, Is.EqualTo(givenEndTime));
            Assert.That(projectedPlayOffers[0].IsCancelled, Is.False);
        });
    }
    
    [Test]
    public async Task PlayOfferCancelledEvent_ProjectionTest()
    {
        //Given
        var existingPlayOffer = (await TestPlayOfferRepository.GetPlayOffersByIds(Guid.Parse("f515dc74-46ad-4a7b-b4f2-8dfda4b7506f"))).First();
        var playOfferCancelledEvent = new TechnicalPlayOfferEvent
        {
            EntityId = existingPlayOffer.Id,
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.NewGuid(),
            EventType = EventType.PLAYOFFER_CANCELLED,
            EventData = new PlayOfferCancelledEvent()
        };
        
        //When
        await Mediator.Send(playOfferCancelledEvent);
        
        //Then
        var projectedPlayOffer = (await TestPlayOfferRepository.GetPlayOffersByIds(Guid.Parse("f515dc74-46ad-4a7b-b4f2-8dfda4b7506f"))).First();
        
        Assert.That(projectedPlayOffer, Is.Not.Null);
        Assert.That(projectedPlayOffer.IsCancelled, Is.True);
    }
    
    [Test]
    public async Task PlayOfferJoinedEvent_ProjectionTest()
    {
        //Given
        var existingPlayOffer = (await TestPlayOfferRepository.GetPlayOffersByIds(Guid.Parse("f515dc74-46ad-4a7b-b4f2-8dfda4b7506f"))).First();
        var givenAcceptedTime = DateTime.UtcNow.AddHours(1);
        var playOfferJoinedEvent = new TechnicalPlayOfferEvent
        {
            EntityId = existingPlayOffer.Id,
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.NewGuid(),
            EventType = EventType.PLAYOFFER_JOINED,
            EventData = new PlayOfferJoinedEvent
            {
                OpponentId = Guid.Parse("f2d11d00-305e-492a-adc0-0e8e63328d66"),
                AcceptedStartTime = givenAcceptedTime
            }
        };
        
        //When
        await Mediator.Send(playOfferJoinedEvent);
        
        //Then
        var projectedPlayOffer = (await TestPlayOfferRepository.GetPlayOffersByIds(Guid.Parse("f515dc74-46ad-4a7b-b4f2-8dfda4b7506f"))).First();
        
        Assert.That(projectedPlayOffer, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedPlayOffer.AcceptedStartTime, Is.EqualTo(givenAcceptedTime));
            Assert.That(projectedPlayOffer.OpponentId, Is.EqualTo(Guid.Parse("f2d11d00-305e-492a-adc0-0e8e63328d66")));
        });
    }
    
    [Test]
    public async Task PlayOfferOpponentRemovedEvent_ProjectionTest()
    {
        //Given
        var existingPlayOffer = (await TestPlayOfferRepository.GetPlayOffersByIds(Guid.Parse("fd385a56-17b1-4318-bc46-cf7f75f20283"))).First();
        var playOfferOpponentRemovedEvent = new TechnicalPlayOfferEvent
        {
            EntityId = existingPlayOffer.Id,
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.NewGuid(),
            EventType = EventType.PLAYOFFER_OPPONENT_REMOVED,
            EventData = new PlayOfferOpponentRemovedEvent()
        };
        
        //When
        await Mediator.Send(playOfferOpponentRemovedEvent);
        
        //Then
        var projectedPlayOffer = (await TestPlayOfferRepository.GetPlayOffersByIds(Guid.Parse("fd385a56-17b1-4318-bc46-cf7f75f20283"))).First();
        
        Assert.That(projectedPlayOffer, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedPlayOffer.OpponentId, Is.Null);
            Assert.That(projectedPlayOffer.AcceptedStartTime, Is.Null);
        });
    }

    [Test]
    public async Task PlayOfferReservationAddedEvent_ProjectionTest()
    {
        //Given
        var existingPlayOffer = (await TestPlayOfferRepository.GetPlayOffersByIds(Guid.Parse("f515dc74-46ad-4a7b-b4f2-8dfda4b7506f"))).First();
        var givenReservationId = Guid.NewGuid();
        var playOfferReservationAddedEvent = new TechnicalPlayOfferEvent
        {
            EntityId = existingPlayOffer.Id,
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.NewGuid(),
            EventType = EventType.PLAYOFFER_RESERVATION_ADDED,
            EventData = new PlayOfferReservationAddedEvent
            {
                ReservationId = givenReservationId
            }
        };
        
        //When
        await Mediator.Send(playOfferReservationAddedEvent);
        
        //Then
        var projectedPlayOffer = (await TestPlayOfferRepository.GetPlayOffersByIds(Guid.Parse("f515dc74-46ad-4a7b-b4f2-8dfda4b7506f"))).First();
        
        Assert.That(projectedPlayOffer, Is.Not.Null);
        Assert.That(projectedPlayOffer.ReservationId, Is.EqualTo(givenReservationId));
    }
}