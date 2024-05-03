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
        
        var club = new Club();
        club.Apply(events);

        return club;
    }
}
