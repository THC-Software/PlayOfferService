using System.Data;
using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Domain.Repositories;
using PlayOfferService.Models;

namespace PlayOfferService.Repositories;

public class MemberRepository
{
    private readonly DbReadContext _context;

    public MemberRepository(DbReadContext context)
    {
        _context = context;
    }

    public async Task<Member> GetMemberById(Guid? memberId)
    {
        var member = await _context.Members
            .Where(e => e.Id == memberId)
            .ToListAsync();

        if (member.Count == 0)
        {
            throw new ArgumentException("No member found with id " + memberId);
        }

        return member.First();
    }

    public async Task UpdateEntityAsync(BaseEvent baseEvent)
    {
        Console.WriteLine("MemberRepository received event: " + baseEvent.EventType);
        var existingMember = await GetMemberById(baseEvent.EntityId);
        
        var appliedEvents = await _context.AppliedEvents
            .Where(e => e.EntityId == baseEvent.EntityId)
            .ToListAsync();
        
        if (appliedEvents.Any(e => e.EventId == baseEvent.EventId))
        {
            Console.WriteLine("Event already applied, skipping");
            return;
        }
        
        existingMember.Apply([baseEvent]);
        _context.AppliedEvents.Add(baseEvent);
        await _context.SaveChangesAsync();
    }
}