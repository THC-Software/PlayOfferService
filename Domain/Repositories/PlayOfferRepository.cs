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
        Guid? clubId = null,
        Guid? opponentId = null)
    {
        var playOffers = await _context.PlayOffers
            .ToListAsync();

        playOffers = playOffers.Where(e =>
            e != null
            && (!playOfferId.HasValue || e.Id == playOfferId)
            && (!creatorId.HasValue || e.CreatorId == creatorId)
            && (!clubId.HasValue || e.ClubId == clubId)
            && (!opponentId.HasValue || e.OpponentId == opponentId)
            ).ToList();

        return playOffers;
    }
    
    public async Task<List<PlayOffer>> GetPlayOffersByParticipantId(Guid participantId)
    {
        var playOffers = await _context.PlayOffers.Where(e => e.CreatorId == participantId || e.OpponentId == participantId).ToListAsync();

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
    
    public async Task<PlayOffer?> GetPlayOfferByReservationId(Guid reservationId)
    {
        return await _context.PlayOffers
            .Where(e => e.ReservationId == reservationId)
            .FirstOrDefaultAsync();
    }


    public async Task Update()
    {
        await _context.SaveChangesAsync();
    }
    
    public void CreatePlayOffer(PlayOffer playOffer)
    {
        _context.PlayOffers.Add(playOffer);
    }
}