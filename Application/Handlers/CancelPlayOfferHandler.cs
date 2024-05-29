using MediatR;
using PlayOfferService.Application.Exceptions;
using PlayOfferService.Commands;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Repositories;
using PlayOfferService.Models;

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
        var existingPlayOffer = (await _playOfferRepository.GetPlayOffersByIds(request.playOfferId)).FirstOrDefault();
        if (existingPlayOffer == null)
            throw new NotFoundException($"PlayOffer {request.playOfferId} not found!");
        if (existingPlayOffer.Opponent != null)
            throw new InvalidOperationException($"PlayOffer {request.playOfferId} is already accepted and cannot be cancelled!");
        if (existingPlayOffer.IsCancelled)
            throw new InvalidOperationException($"PlayOffer {request.playOfferId} is already cancelled!");
        
        switch (existingPlayOffer.Club.Status)
        {
            case Status.LOCKED:
                throw new InvalidOperationException("Can't cancel PlayOffer while club is locked!");
            case Status.DELETED:
                throw new InvalidOperationException("Can't cancel PlayOffer in deleted club!");
        }
        
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