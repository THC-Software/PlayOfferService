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

    public async Task UpdateEntityAsync(BaseEvent baseEvent)
    {
        Console.WriteLine("MemberRepo received event: " + baseEvent.EventType);
        var existingClub = await GetClubById(baseEvent.EntityId);
        
        var appliedEvents = await _context.AppliedEvents
            .Where(e => e.EntityId == baseEvent.EntityId)
            .ToListAsync();
        
        if (appliedEvents.Any(e => e.EventId == baseEvent.EventId))
        {
            Console.WriteLine("Event already applied, skipping");
            return;
        }
        
        existingClub.Apply([baseEvent]);
        _context.AppliedEvents.Add(baseEvent);
        await _context.SaveChangesAsync();
    }
}
