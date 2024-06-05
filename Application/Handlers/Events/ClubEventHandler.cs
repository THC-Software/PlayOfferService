using MediatR;
using PlayOfferService.Application.Exceptions;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers.Events;

public class ClubEventHandler : IRequestHandler<ClubBaseEvent>
{
    private readonly ClubRepository _clubRepository;
    private readonly ReadEventRepository _eventRepository;
    
    public ClubEventHandler(ClubRepository clubRepository, ReadEventRepository eventRepository)
    {
        _clubRepository = clubRepository;
        _eventRepository = eventRepository;
    }
    
    public async Task Handle(ClubBaseEvent request, CancellationToken cancellationToken)
    {
        Console.WriteLine("ClubEventHandler received event: " + request.EventType);
        var existingEvent = await _eventRepository.GetEventById(request.EventId);
        if (existingEvent != null)
        {
            Console.WriteLine("Event already applied, skipping");
            return;
        }
        
        switch (request.EventType)
        {
            case EventType.TENNIS_CLUB_REGISTERED:
                await HandleTennisClubRegisteredEvent(request);
                break;
            case EventType.TENNIS_CLUB_LOCKED:
                await HandleTennisClubLockedEvent(request);
                break;
            case EventType.TENNIS_CLUB_UNLOCKED:
                await HandleTennisClubUnlockedEvent(request);
                break;
            case EventType.TENNIS_CLUB_DELETED:
                await HandleTennisClubDeletedEvent(request);
                break;
        }
        
        await _clubRepository.Update();
        await _eventRepository.AppendEvent(request);
        await _eventRepository.Update();
    }

    private async Task HandleTennisClubDeletedEvent(ClubBaseEvent baseEvent)
    {
        var existingClub = await _clubRepository.GetClubById(baseEvent.EntityId);
        existingClub!.Apply([baseEvent]);
    }

    private async Task HandleTennisClubUnlockedEvent(ClubBaseEvent baseEvent)
    {
        var existingClub = await _clubRepository.GetClubById(baseEvent.EntityId);
        existingClub!.Apply([baseEvent]);
    }

    private async Task HandleTennisClubLockedEvent(ClubBaseEvent baseEvent)
    {
        var existingClub = await _clubRepository.GetClubById(baseEvent.EntityId);
        existingClub!.Apply([baseEvent]);
    }

    private async Task HandleTennisClubRegisteredEvent(ClubBaseEvent baseEvent)
    {
        var newClub = new Club();
        newClub.Apply([baseEvent]);
        _clubRepository.CreateClub(newClub);
    }
}