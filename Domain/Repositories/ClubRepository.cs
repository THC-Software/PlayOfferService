using System.Data;
using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Repositories;
using PlayOfferService.Models;

namespace PlayOfferService.Repositories;

public class ClubRepository
{
    private readonly DbReadContext _context;
    
    public ClubRepository(DbReadContext context)
    {
        _context = context;
    }
    
    public async Task<Club> GetClubById(Guid clubId)
    {
        var club = await _context.Clubs
            .Where(e => e.Id == clubId)
            .ToListAsync();

        if (club.Count == 0)
        {
            throw new ArgumentException("No club found with id " + clubId);
        }

        return club.First();
    }

    public async Task UpdateEntityAsync(BaseEvent parsedEvent)
    {
        throw new NotImplementedException();
    }
}
