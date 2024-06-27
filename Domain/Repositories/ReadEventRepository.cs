using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;

namespace PlayOfferService.Domain.Repositories;

public class ReadEventRepository
{
    private readonly DbReadContext _context;
    
    public ReadEventRepository(){}
    
    public ReadEventRepository(DbReadContext context)
    {
        _context = context;
    }
    
    public virtual async Task<BaseEvent?> GetEventById(Guid eventId)
    {
        var events = await _context.AppliedEvents
            .Where(e => e.EventId == eventId)
            .ToListAsync();

        if (events.Count == 0)
            return null;

        return events.First();
    }
    
    public virtual async Task AppendEvent(BaseEvent baseEvent)
    {
        _context.AppliedEvents.Add(baseEvent);
    }
    
    public virtual async Task Update()
    {
        await _context.SaveChangesAsync();
    }
}