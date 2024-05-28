using MediatR;
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
            throw new ArgumentException("PlayOffer not found with id: " + request.playOfferId);
        
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