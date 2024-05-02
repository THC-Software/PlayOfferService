using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Models;

namespace PlayOfferService.Repositories;

public class PlayOfferRepository
{
    private readonly DatabaseContext _context;
    
    public PlayOfferRepository(DatabaseContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<PlayOffer>> GetPlayOffersById(Guid? playOfferId, Guid? creatorId, Guid? clubId)
    {
        var playOfferCreatedEvents = await _context.Events.Where(e =>
            e.EventType == EventType.PLAYOFFER_CREATED
        )
            .Include(e => e.EventData)
            .Include(e => e.EventData)
            .Include(e => ((PlayOfferCreatedEvent)e.EventData).Club)
            .Include(e => ((PlayOfferCreatedEvent)e.EventData).Creator)
            .ToListAsync();

        playOfferCreatedEvents = playOfferCreatedEvents.Where(e =>
            e != null
            && (!playOfferId.HasValue || e.EntityId == playOfferId)
            && (!creatorId.HasValue || ((PlayOfferCreatedEvent)e.EventData).Creator.Id == creatorId)
            && (!clubId.HasValue || ((PlayOfferCreatedEvent)e.EventData).Club.Id == clubId)).ToList();
        
        var result = new List<PlayOffer>();
        foreach (var group in playOfferCreatedEvents.GroupBy(e => e.EntityId))
        {
            Guid entityId = group.Key;
            var eventsForPlayOffer = await _context.Events.Where(e => e.EntityId == entityId).ToListAsync();
            
            var playOffer = new PlayOffer(entityId);
            foreach (var baseEvent in eventsForPlayOffer)
            {
                playOffer.Apply(baseEvent);
            }
            result.Add(playOffer);
        }

        return result;
    }
}