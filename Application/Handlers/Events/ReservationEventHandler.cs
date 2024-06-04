using MediatR;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.PlayOffer;
using PlayOfferService.Domain.Events.Reservation;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers.Events;

public class ReservationEventHandler : IRequestHandler<ReservationBaseEvent>
{
    private readonly DbWriteContext _writeContext;
    private readonly PlayOfferRepository _playOfferRepository;
    
    public ReservationEventHandler(DbWriteContext context, PlayOfferRepository playOfferRepository)
    {
        _writeContext = context;
        _playOfferRepository = playOfferRepository;
    }

    public async Task Handle(ReservationBaseEvent baseEvent, CancellationToken cancellationToken)
    {
        switch (baseEvent.EventType)
        {
            case EventType.ReservationCreatedEvent:
                await HandleReservationCreatedEvent(baseEvent);
                break;
            case EventType.ReservationRejectedEvent:
                await HandleReservationRejectedEvent(baseEvent);
                break;
        }
    }

    private async Task HandleReservationRejectedEvent(ReservationBaseEvent baseEvent)
    {
        var existingPlayOffer = await _playOfferRepository.GetPlayOfferByEventId((Guid)baseEvent.CorrelationId!);
        if (existingPlayOffer == null)
            return;
        
        var playOfferEvent = new BaseEvent
        {
            EventId = Guid.NewGuid(),
            EntityId = existingPlayOffer.Id,
            EventType = EventType.PLAYOFFER_OPPONENT_REMOVED,
            EntityType = EntityType.PLAYOFFER,
            Timestamp = DateTime.UtcNow,
            CorrelationId = baseEvent.EventId,
            EventData = new PlayOfferOpponentRemovedEvent()
        };

        _writeContext.Events.Add(playOfferEvent);
        await _writeContext.SaveChangesAsync();
        
        // TODO: Implement reservation read side projection
    }
    
    private async Task HandleReservationCreatedEvent(ReservationBaseEvent baseEvent)
    {
        // Check if ReservationCreatedEvent is a response to a PlayOfferJoinedEvent
        if (baseEvent.CorrelationId != null)
        {
            var existingPlayOffer = await _playOfferRepository.GetPlayOfferByEventId((Guid)baseEvent.CorrelationId!);
            if (existingPlayOffer == null)
                return;
        
            var playOfferEvent = new BaseEvent
            {
                EventId = Guid.NewGuid(),
                EntityId = existingPlayOffer.Id,
                EventType = EventType.PLAYOFFER_RESERVATION_ADDED,
                EntityType = EntityType.PLAYOFFER,
                Timestamp = DateTime.UtcNow,
                CorrelationId = baseEvent.EventId,
                EventData = new PlayOfferReservationAddedEvent{ReservationId = baseEvent.EntityId}
            };

            _writeContext.Events.Add(playOfferEvent);
            await _writeContext.SaveChangesAsync();
        }
        
        // TODO: Implement reservation read side projection
    }
}