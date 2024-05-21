using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Repositories;
using PlayOfferService.Models;

namespace PlayOfferService.Repositories;

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

    public Task UpdateEntityAsync(BaseEvent baseEvent)
    {
        Console.WriteLine("PlayOfferRepo received event: " + baseEvent.EventType);
        return Task.CompletedTask;
    }
}