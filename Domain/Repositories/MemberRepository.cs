using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Repositories;

public class MemberRepository
{
    private readonly DbReadContext _context;
    
    public MemberRepository(){}

    public MemberRepository(DbReadContext context)
    {
        _context = context;
    }

    public virtual async Task<Member?> GetMemberById(Guid? memberId)
    {
        var member = await _context.Members
            .Where(e => e.Id == memberId)
            .ToListAsync();

        if (member.Count == 0)
            return null;

        return member.First();
    }

    public async Task UpdateEntityAsync(BaseEvent baseEvent)
    {
        Console.WriteLine("MemberRepository received event: " + baseEvent.EventType);
        var appliedEvents = await _context.AppliedEvents
            .Where(e => e.EntityId == baseEvent.EntityId)
            .ToListAsync();
        
        if (appliedEvents.Any(e => e.EventId == baseEvent.EventId))
        {
            Console.WriteLine("Event already applied, skipping");
            return;
        }
        
        if (baseEvent.EventType == EventType.MEMBER_REGISTERED)
        {
            var newMember = new Member();
            newMember.Apply([baseEvent]);
            _context.Members.Add(newMember);
        }
        else
        {
            var existingMember = await GetMemberById(baseEvent.EntityId);
            existingMember?.Apply([baseEvent]);
        }

        _context.AppliedEvents.Add(baseEvent);
        await _context.SaveChangesAsync();
    }
}