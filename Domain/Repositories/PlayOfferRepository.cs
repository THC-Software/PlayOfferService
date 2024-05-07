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
    
    public async Task<IEnumerable<PlayOffer>> GetPlayOffersByIds(
        Guid? playOfferId,
        Guid? creatorId = null,
        Guid? clubId = null)
    {
        var playOfferCreatedEvents = await _context.Events.Where(e =>
            e.EventType == EventType.PLAYOFFER_CREATED
        )
            .ToListAsync();

        playOfferCreatedEvents = playOfferCreatedEvents.Where(e =>
            e != null
            && (!playOfferId.HasValue || e.EntityId == playOfferId)
            && (!creatorId.HasValue || ((PlayOfferCreatedEvent)e.EventData).Creator.Id == creatorId)
            && (!clubId.HasValue || ((PlayOfferCreatedEvent)e.EventData).Club.Id == clubId)).ToList();
        
        if(playOfferCreatedEvents.Count == 0)
        {
            return new List<PlayOffer>();
        }
        
        var result = new List<PlayOffer>();
        foreach (var group in playOfferCreatedEvents.GroupBy(e => e.EntityId))
        {
            Guid entityId = group.Key;
            var eventsForPlayOffer = group.ToList();
            eventsForPlayOffer.AddRange(await _context.Events.Where(e => e.EntityId == entityId).ToListAsync());

            if (eventsForPlayOffer.Count != 0)
            {
                var playOffer = new PlayOffer();
                playOffer.Apply(eventsForPlayOffer);
                result.Add(playOffer);
            }           

        }

        return result;
    }

    public Task UpdateEntityAsync(BaseEvent baseEvent)
    {
        Console.WriteLine("PlayOfferRepo received event: " + baseEvent.EventType);
        return Task.CompletedTask;
    }
}