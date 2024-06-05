using MediatR;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.PlayOffer;
using PlayOfferService.Domain.Events.Reservation;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers.Events;

public class ReservationEventHandler : IRequestHandler<TechnicalReservationEvent>
{
    private readonly DbWriteContext _writeContext;
    private readonly PlayOfferRepository _playOfferRepository;
    private readonly ReadEventRepository _eventRepository;
    
    public ReservationEventHandler(DbWriteContext context, PlayOfferRepository playOfferRepository, ReadEventRepository eventRepository)
    {
        _writeContext = context;
        _playOfferRepository = playOfferRepository;
        _eventRepository = eventRepository;
    }

    public async Task Handle(TechnicalReservationEvent reservationEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine("ReservationEventHandler received event: " + reservationEvent.EventType);
        var existingEvent = await _writeContext.Events.FindAsync(reservationEvent.EventId);
        if (existingEvent != null)
        {
            Console.WriteLine("Event already applied, skipping");
            return;
        }
        
        switch (reservationEvent.EventType)
        {
            case EventType.ReservationCreatedEvent:
                await HandleReservationCreatedEvent(reservationEvent);
                break;
            case EventType.ReservationRejectedEvent:
                await HandleReservationRejectedEvent(reservationEvent);
                break;
        }
        
        await _eventRepository.AppendEvent(reservationEvent);
        await _eventRepository.Update();
    }

    private async Task HandleReservationRejectedEvent(TechnicalReservationEvent reservationEvent)
    {
        var existingPlayOffer = await _playOfferRepository.GetPlayOfferByEventId((Guid)reservationEvent.CorrelationId!);
        if (existingPlayOffer == null)
            return;
        
        var playOfferEvent = new BaseEvent
        {
            EventId = Guid.NewGuid(),
            EntityId = existingPlayOffer.Id,
            EventType = EventType.PLAYOFFER_OPPONENT_REMOVED,
            EntityType = EntityType.PLAYOFFER,
            Timestamp = DateTime.UtcNow,
            CorrelationId = reservationEvent.EventId,
            EventData = new PlayOfferOpponentRemovedEvent()
        };

        _writeContext.Events.Add(playOfferEvent);
        await _writeContext.SaveChangesAsync();
        
        // TODO: Implement reservation read side projection
    }
    
    private async Task HandleReservationCreatedEvent(TechnicalReservationEvent reservationEvent)
    {
        // Check if ReservationCreatedEvent is a response to a PlayOfferJoinedEvent
        if (reservationEvent.CorrelationId != null)
        {
            var existingPlayOffer = await _playOfferRepository.GetPlayOfferByEventId((Guid)reservationEvent.CorrelationId!);
            if (existingPlayOffer == null)
                return;
        
            var playOfferEvent = new BaseEvent
            {
                EventId = Guid.NewGuid(),
                EntityId = existingPlayOffer.Id,
                EventType = EventType.PLAYOFFER_RESERVATION_ADDED,
                EntityType = EntityType.PLAYOFFER,
                Timestamp = DateTime.UtcNow,
                CorrelationId = reservationEvent.EventId,
                EventData = new PlayOfferReservationAddedEvent{ReservationId = reservationEvent.EntityId}
            };

            _writeContext.Events.Add(playOfferEvent);
            await _writeContext.SaveChangesAsync();
        }
        
        // TODO: Implement reservation read side projection
    }
}