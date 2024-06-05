using MediatR;
using PlayOfferService.Application.Exceptions;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers.Events;

public class ClubEventHandler : IRequestHandler<TechnicalClubEvent>
{
    private readonly ClubRepository _clubRepository;
    private readonly ReadEventRepository _eventRepository;
    
    public ClubEventHandler(ClubRepository clubRepository, ReadEventRepository eventRepository)
    {
        _clubRepository = clubRepository;
        _eventRepository = eventRepository;
    }
    
    public async Task Handle(TechnicalClubEvent clubEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine("ClubEventHandler received event: " + clubEvent.EventType);
        var existingEvent = await _eventRepository.GetEventById(clubEvent.EventId);
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
        await _eventRepository.AppendEvent(clubEvent);
        await _eventRepository.Update();
    }

    private async Task HandleTennisClubDeletedEvent(TechnicalClubEvent clubEvent)
    {
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
        var existingClub = await _clubRepository.GetClubById(clubEvent.EntityId);
        existingClub!.Apply([clubEvent]);
    }

    private async Task HandleTennisClubRegisteredEvent(TechnicalClubEvent clubEvent)
    {
        var newClub = new Club();
        newClub.Apply([clubEvent]);
        _clubRepository.CreateClub(newClub);
    }
}