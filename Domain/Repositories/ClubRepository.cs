using System.Data;
using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Models;

namespace PlayOfferService.Repositories;

public class ClubRepository
{
    private readonly DatabaseContext _context;
    
    public ClubRepository(DatabaseContext context)
    {
        _context = context;
    }
    
    public async Task<Club> GetClubById(Guid clubId)
    {
        var events = await _context.Events
            .Where(e => e.EntityId == clubId)
            .OrderBy(e => e.Timestamp)
            .ToListAsync();
        
        if (events.First().EventType != EventType.TENNIS_CLUB_REGISTERED)
        {
            throw new DataException("INVALID STATE: First Club Event must be of type "+nameof(EventType.TENNIS_CLUB_REGISTERED));
        }
        
        var club = new Club();
        foreach (var baseEvent in events)
        {
            club.Apply(baseEvent);
        }
        return club;
    }
}
