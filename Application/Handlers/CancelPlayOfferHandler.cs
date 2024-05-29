using MediatR;
using PlayOfferService.Application.Exceptions;
using PlayOfferService.Commands;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers;

public class CancelPlayOfferHandler : IRequestHandler<CancelPlayOfferCommand, Task>
{
    private readonly DbWriteContext _context;
    private readonly PlayOfferRepository _playOfferRepository;
    
    public CancelPlayOfferHandler(DbWriteContext context, PlayOfferRepository playOfferRepository)
    {
        _context = context;
        _playOfferRepository = playOfferRepository;
    }


    public async Task<Task> Handle(CancelPlayOfferCommand request, CancellationToken cancellationToken)
    {
        var existingPlayOffers = await _playOfferRepository.GetPlayOffersByIds(request.playOfferId);
        if (existingPlayOffers.ToList().Count == 0)
            throw new NotFoundException($"PlayOffer {request.playOfferId} not found!");
        if (existingPlayOffers.First().Opponent != null)
            throw new InvalidOperationException($"PlayOffer {request.playOfferId} is already accepted and cannot be cancelled!");
        if (existingPlayOffers.First().IsCancelled)
            throw new InvalidOperationException($"PlayOffer {request.playOfferId} is already cancelled!");
        
        var domainEvent = new BaseEvent
        {
            EntityId = request.playOfferId,
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.NewGuid(),
            EventType = EventType.PLAYOFFER_CANCELLED,
            EventData = new PlayOfferCancelledEvent(),
            Timestamp = DateTime.UtcNow
        };
        
        _context.Events.Add(domainEvent);
        await _context.SaveChangesAsync();
        
        return Task.CompletedTask;
    }
}