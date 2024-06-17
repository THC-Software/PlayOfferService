using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Domain.Repositories;

public class ClubRepository
{
    private readonly DbReadContext _context;

    public ClubRepository(){}

    public ClubRepository(DbReadContext context)
    {
        _context = context;
    }

    public virtual async Task Update()
    {
        await _context.SaveChangesAsync();
    }
    
    public virtual async Task<Club?> GetClubById(Guid clubId)
    {
        var club = await _context.Clubs
            .Where(e => e.Id == clubId)
            .ToListAsync();

        if (club.Count == 0)
            return null;

        return club.First();
    }
    
    public virtual async Task<IEnumerable<Club>> GetAllClubs()
    {
        return await _context.Clubs.ToListAsync();
    }
    
    public virtual void CreateClub(Club club)
    {
        _context.Clubs.Add(club);
    }
}
