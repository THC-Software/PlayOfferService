using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Models;

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
            .Include(playOffer => playOffer.Creator)
            .Include(playOffer => playOffer.Club)
            .ToListAsync();

        playOffers = playOffers.Where(e =>
            e != null
            && (!playOfferId.HasValue || e.Id == playOfferId)
            && (!creatorId.HasValue || e.Creator.Id == creatorId)
            && (!clubId.HasValue || e.Club.Id == clubId)).ToList();

        return playOffers;
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
        
        if (baseEvent.EventType == EventType.PLAYOFFER_CREATED)
        {
            var newPlayOffer = new PlayOffer();
            newPlayOffer.Apply([baseEvent]);
            _context.PlayOffers.Add(newPlayOffer);
        }
        else
        {
            var existingPlayOffer = (await GetPlayOffersByIds(baseEvent.EntityId)).First();
            existingPlayOffer.Apply([baseEvent]);
        }
        _context.AppliedEvents.Add(baseEvent);
        await _context.SaveChangesAsync();
    }
}