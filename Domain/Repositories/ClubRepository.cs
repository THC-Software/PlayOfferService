using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Repositories;

public class ClubRepository
{
    private readonly DbReadContext _context;

    public ClubRepository(){}

    public ClubRepository(DbReadContext context)
    {
        _context = context;
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

    public async Task UpdateEntityAsync(BaseEvent baseEvent)
    {
        Console.WriteLine("MemberRepo received event: " + baseEvent.EventType);
        var appliedEvents = await _context.AppliedEvents
            .Where(e => e.EntityId == baseEvent.EntityId)
            .ToListAsync();
        
        if (appliedEvents.Any(e => e.EventId == baseEvent.EventId))
        {
            Console.WriteLine("Event already applied, skipping");
            return;
        }
        
        if (baseEvent.EventType == EventType.TENNIS_CLUB_REGISTERED)
        {
            var newClub = new Club();
            newClub.Apply([baseEvent]);
            _context.Clubs.Add(newClub);
        }
        else
        {
            var existingClub = await GetClubById(baseEvent.EntityId);
            existingClub.Apply([baseEvent]);
        }
        
        _context.AppliedEvents.Add(baseEvent);
        await _context.SaveChangesAsync();
    }
}
