using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Domain.Repositories;

public class PlayOfferRepository
{
    private readonly DbReadContext _context;

    public PlayOfferRepository(DbReadContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PlayOffer>> GetPlayOffersByIds(
        Guid? playOfferId,
        Guid? creatorId = null,
        Guid? clubId = null)
    {
        var playOffers = await _context.PlayOffers
            .ToListAsync();

        playOffers = playOffers.Where(e =>
            e != null
            && (!playOfferId.HasValue || e.Id == playOfferId)
            && (!creatorId.HasValue || e.CreatorId == creatorId)
            && (!clubId.HasValue || e.ClubId == clubId)).ToList();

        return playOffers;
    }
    
    public async Task<PlayOffer?> GetPlayOfferByEventId(Guid eventId)
    {
        var playOffer = await _context.AppliedEvents
            .Where(e => e.EventId == eventId)
            .Select(e => e.EntityId)
            .SelectMany(e => _context.PlayOffers.Where(p => p.Id == e))
            .FirstOrDefaultAsync();

        return playOffer;
    }

    public async Task UpdateEntityAsync(BaseEvent baseEvent)
    {
        Console.WriteLine("PlayOfferRepository received event: " + baseEvent.EventType);
        var appliedEvents = await _context.AppliedEvents
            .Where(e => e.EntityId == baseEvent.EntityId)
            .ToListAsync();

        if (appliedEvents.Any(e => e.EventId == baseEvent.EventId))
        {
            Console.WriteLine("Event already applied, skipping");
            return;
        }
        
        if (baseEvent.EntityType == EntityType.PLAYOFFER) {
            _context.AppliedEvents.Add(baseEvent);
        }

        switch (baseEvent.EventType)
        {
            case EventType.PLAYOFFER_CANCELLED:
                await CancelPlayOffer(baseEvent);
                break;
            case EventType.PLAYOFFER_CREATED:
                CreatePlayOffer(baseEvent);
                break;
            case EventType.PLAYOFFER_JOINED:
                await JoinPlayOffer(baseEvent);
                break;
            case EventType.ReservationCreatedEvent:
                await ReservationCreated(baseEvent);
                break;
        }
        
        await _context.SaveChangesAsync();
    }

    private async Task CancelPlayOffer(BaseEvent baseEvent)
    {
        var existingPlayOffer = (await GetPlayOffersByIds(baseEvent.EntityId)).First();
        existingPlayOffer.Apply([baseEvent]);
    }

    private void CreatePlayOffer(BaseEvent baseEvent)
    {
        var newPlayOffer = new PlayOffer();
        newPlayOffer.Apply([baseEvent]);
        _context.PlayOffers.Add(newPlayOffer);
    }

    private async Task JoinPlayOffer(BaseEvent baseEvent)
    {
        var existingPlayOffer = (await GetPlayOffersByIds(baseEvent.EntityId)).First();
        existingPlayOffer.Apply([baseEvent]);
    }
    
    private async Task ReservationCreated(BaseEvent baseEvent)
    {
        if (baseEvent.CorrelationId == null)
            throw new ArgumentException("CorrelationId is null for ReservationCreatedEvent");
        
        var existingPlayOffer = await GetPlayOfferByEventId((Guid)baseEvent.CorrelationId);
        if (existingPlayOffer == null)
            throw new ArgumentException($"PlayOffer not found for ReservationCreatedEvent");
        
        existingPlayOffer.Apply([baseEvent]);
    }
}