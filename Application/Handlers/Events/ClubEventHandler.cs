using MediatR;
using PlayOfferService.Application.Exceptions;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.PlayOffer;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers.Events;

public class ClubEventHandler : IRequestHandler<TechnicalClubEvent>
{
    private readonly ClubRepository _clubRepository;
    private readonly ReadEventRepository _readEventRepository;
    private readonly WriteEventRepository _writeEventRepository;
    private readonly PlayOfferRepository _playOfferRepository;
    
    public ClubEventHandler(ClubRepository clubRepository, ReadEventRepository readEventRepository, WriteEventRepository writeEventRepository, PlayOfferRepository playOfferRepository)
    {
        _clubRepository = clubRepository;
        _readEventRepository = readEventRepository;
        _writeEventRepository = writeEventRepository;
        _playOfferRepository = playOfferRepository;
    }
    
    public async Task Handle(TechnicalClubEvent clubEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine("ClubEventHandler received event: " + clubEvent.EventType);
        var existingEvent = await _readEventRepository.GetEventById(clubEvent.EventId);
        if (existingEvent != null)
        {
            Console.WriteLine("Event already applied, skipping");
            return;
        }
        
        switch (clubEvent.EventType)
        {
            case EventType.TENNIS_CLUB_REGISTERED:
                await HandleTennisClubRegisteredEvent(clubEvent);
                break;
            case EventType.TENNIS_CLUB_LOCKED:
                await HandleTennisClubLockedEvent(clubEvent);
                break;
            case EventType.TENNIS_CLUB_UNLOCKED:
                await HandleTennisClubUnlockedEvent(clubEvent);
                break;
            case EventType.TENNIS_CLUB_DELETED:
                await HandleTennisClubDeletedEvent(clubEvent);
                break;
        }
        
        await _clubRepository.Update();
        await _readEventRepository.AppendEvent(clubEvent);
        await _readEventRepository.Update();
    }

    private async Task HandleTennisClubDeletedEvent(TechnicalClubEvent clubEvent)
    {
        await CreatePlayOfferCancelledEventsByClubId(clubEvent);
        
        var existingClub = await _clubRepository.GetClubById(clubEvent.EntityId);
        existingClub!.Apply([clubEvent]);
    }

    private async Task HandleTennisClubUnlockedEvent(TechnicalClubEvent clubEvent)
    {
        var existingClub = await _clubRepository.GetClubById(clubEvent.EntityId);
        existingClub!.Apply([clubEvent]);
    }

    private async Task HandleTennisClubLockedEvent(TechnicalClubEvent clubEvent)
    {
        await CreatePlayOfferCancelledEventsByClubId(clubEvent);
        
        var existingClub = await _clubRepository.GetClubById(clubEvent.EntityId);
        existingClub!.Apply([clubEvent]);
    }

    private async Task HandleTennisClubRegisteredEvent(TechnicalClubEvent clubEvent)
    {
        var newClub = new Club();
        newClub.Apply([clubEvent]);
        _clubRepository.CreateClub(newClub);
    }
    
    private async Task CreatePlayOfferCancelledEventsByClubId(TechnicalClubEvent clubEvent)
    {
        // Get all play offers by club id
        var existingPlayOffer = await _playOfferRepository.GetPlayOffersByIds(null, null, clubEvent.EntityId);
        
        // Create PlayOfferCancelled events for each play offer
        foreach (var playOffer in existingPlayOffer)
        {
            var cancelledEvent = new BaseEvent
            {
                EventId = Guid.NewGuid(),
                EventType = EventType.PLAYOFFER_CANCELLED,
                EntityType = EntityType.PLAYOFFER,
                EntityId = playOffer.Id,
                Timestamp = DateTime.UtcNow,
                EventData = new PlayOfferCancelledEvent(),
                CorrelationId = clubEvent.EventId
            };
            
            await _writeEventRepository.AppendEvent(cancelledEvent);
        }
        await _writeEventRepository.Update();
    }
}