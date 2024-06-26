using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PlayOfferService.Domain.Events;

namespace PlayOfferService.Domain.Repositories;

public class WriteEventRepository
{
    private readonly DbWriteContext _context;

    public WriteEventRepository() { }

    public WriteEventRepository(DbWriteContext context)
    {
        _context = context;
    }

    public virtual async Task<BaseEvent?> GetEventById(Guid eventId)
    {
        var events = await _context.Events
            .Where(e => e.EventId == eventId)
            .ToListAsync();

        if (events.Count == 0)
            return null;

        return events.First();
    }

    public virtual async Task<List<BaseEvent>> GetEventByEntityId(Guid entityId)
    {
        var events = await _context.Events
            .Where(e => e.EntityId == entityId)
            .ToListAsync();

        return events;
    }

    public virtual async Task AppendEvent(BaseEvent baseEvent)
    {
        _context.Events.Add(baseEvent);
    }

    public virtual async Task Update()
    {
        await _context.SaveChangesAsync();
    }

    internal int GetEventCount(Guid playOfferId)
    {
        return _context.Events.Count(e => e.EntityId == playOfferId);
    }

    internal IDbContextTransaction StartTransaction()
    {
        return _context.Database.BeginTransaction();
    }
}