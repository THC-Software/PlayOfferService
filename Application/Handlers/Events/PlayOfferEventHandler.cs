using MediatR;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.PlayOffer;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers.Events;

public class PlayOfferEventHandler : IRequestHandler<TechnicalPlayOfferEvent>
{
    private readonly PlayOfferRepository _playOfferRepository;
    private readonly ReadEventRepository _eventRepository;
    
    public PlayOfferEventHandler(PlayOfferRepository playOfferRepository, ReadEventRepository eventRepository)
    {
        _playOfferRepository = playOfferRepository;
        _eventRepository = eventRepository;
    }
    
    public async Task Handle(TechnicalPlayOfferEvent playOfferEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine("PlayOfferEventHandler received event: " + playOfferEvent.EventType);
        var existingEvent = await _eventRepository.GetEventById(playOfferEvent.EventId);
        if (existingEvent != null)
        {
            Console.WriteLine("Event already applied, skipping");
            return;
        }
        
        switch (playOfferEvent.EventType)
        {
            case EventType.PLAYOFFER_CREATED:
                await HandlePlayOfferCreatedEvent(playOfferEvent);
                break;
            case EventType.PLAYOFFER_CANCELLED:
                await HandlePlayOfferCancelledEvent(playOfferEvent);
                break;
            case EventType.PLAYOFFER_JOINED:
                await HandlePlayOfferJoinedEvent(playOfferEvent);
                break;
            case EventType.PLAYOFFER_OPPONENT_REMOVED:
                await HandlePlayOfferOpponentRemovedEvent(playOfferEvent);
                break;
            case EventType.PLAYOFFER_RESERVATION_ADDED:
                await HandlePlayOfferReservationAddedEvent(playOfferEvent);
                break;
        }
        
        await _playOfferRepository.Update();
        await _eventRepository.AppendEvent(playOfferEvent);
        await _eventRepository.Update();
    }

    private async Task HandlePlayOfferReservationAddedEvent(TechnicalPlayOfferEvent playOfferEvent)
    {
        var existingPlayOffer = (await _playOfferRepository.GetPlayOffersByIds(playOfferEvent.EntityId)).First();
        existingPlayOffer.Apply([playOfferEvent]);
    }

    private async Task HandlePlayOfferOpponentRemovedEvent(TechnicalPlayOfferEvent playOfferEvent)
    {
        var existingPlayOffer = (await _playOfferRepository.GetPlayOffersByIds(playOfferEvent.EntityId)).First();
        existingPlayOffer.Apply([playOfferEvent]);
    }

    private async Task HandlePlayOfferJoinedEvent(TechnicalPlayOfferEvent playOfferEvent)
    {
        var existingPlayOffer = (await _playOfferRepository.GetPlayOffersByIds(playOfferEvent.EntityId)).First();
        existingPlayOffer.Apply([playOfferEvent]);
    }

    private async Task HandlePlayOfferCancelledEvent(TechnicalPlayOfferEvent playOfferEvent)
    {
        var existingPlayOffer = (await _playOfferRepository.GetPlayOffersByIds(playOfferEvent.EntityId)).First();
        existingPlayOffer.Apply([playOfferEvent]);
    }

    private async Task HandlePlayOfferCreatedEvent(TechnicalPlayOfferEvent playOfferEvent)
    {
        var newPlayOffer = new PlayOffer();
        newPlayOffer.Apply([playOfferEvent]);
        _playOfferRepository.CreatePlayOffer(newPlayOffer);
    }
}