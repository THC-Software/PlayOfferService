using MediatR;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.PlayOffer;
using PlayOfferService.Domain.Events.Reservation;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers.Events;

public class ReservationEventHandler : IRequestHandler<TechnicalReservationEvent>
{
    private readonly DbWriteContext _writeContext;
    private readonly PlayOfferRepository _playOfferRepository;
    private readonly ReadEventRepository _eventRepository;
    private readonly ReservationRepository _reservationRepository;
    
    public ReservationEventHandler(DbWriteContext context, PlayOfferRepository playOfferRepository, ReadEventRepository eventRepository, ReservationRepository reservationRepository)
    {
        _writeContext = context;
        _playOfferRepository = playOfferRepository;
        _eventRepository = eventRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task Handle(TechnicalReservationEvent reservationEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine("ReservationEventHandler received event: " + reservationEvent.EventType);
        var existingEvent = await _eventRepository.GetEventById(reservationEvent.EventId);
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
            case EventType.ReservationLimitExceeded:
                await HandleReservationLimitExceededEvent(reservationEvent);
                break;
            case EventType.ReservationCancelledEvent:
                await HandleReservationCancelledEvent(reservationEvent);
                break;
        }
        
        await _reservationRepository.Update();
        await _eventRepository.AppendEvent(reservationEvent);
        await _eventRepository.Update();
    }

    private async Task HandleReservationCancelledEvent(TechnicalReservationEvent reservationEvent)
    {
        var existingReservation = await _reservationRepository.GetReservationById(reservationEvent.EntityId);
        existingReservation!.Apply([reservationEvent]);
    }

    private async Task HandleReservationLimitExceededEvent(TechnicalReservationEvent reservationEvent)
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

        var reservation = new Reservation();
        reservation.Apply([reservationEvent]);
        _reservationRepository.CreateReservation(reservation);
    }
}