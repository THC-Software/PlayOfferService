using MediatR;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Court;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers.Events;

public class CourtEventHandler : IRequestHandler<TechnicalCourtEvent>
{
    private readonly CourtRepository _courtRepository;
    private readonly ReadEventRepository _eventRepository;
    
    public CourtEventHandler(CourtRepository courtRepository, ReadEventRepository eventRepository)
    {
        _courtRepository = courtRepository;
        _eventRepository = eventRepository;
    }
    
    public async Task Handle(TechnicalCourtEvent courtEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine("ReservationEventHandler received event: " + courtEvent.EventType);
        var existingEvent = await _eventRepository.GetEventById(courtEvent.EventId);
        if (existingEvent != null)
        {
            Console.WriteLine("Event already applied, skipping");
            return;
        }
        
        switch (courtEvent.EventType)
        {
            case EventType.CourtCreatedEvent:
                await HandleCourtCreatedEvent(courtEvent);
                break;
            case EventType.CourtUpdatedEvent:
                await HandleCourtUpdatedEvent(courtEvent);
                break;
        }
        
        await _courtRepository.Update();
        await _eventRepository.AppendEvent(courtEvent);
        await _eventRepository.Update();
    }

    private async Task HandleCourtUpdatedEvent(TechnicalCourtEvent courtEvent)
    {
        var existingCourt = await _courtRepository.GetCourtById(courtEvent.EntityId);
        existingCourt!.Apply([courtEvent]);
    }

    private async Task HandleCourtCreatedEvent(TechnicalCourtEvent courtEvent)
    {
        var court = new Court();
        court.Apply([courtEvent]);
        _courtRepository.CreateCourt(court);
    }
}