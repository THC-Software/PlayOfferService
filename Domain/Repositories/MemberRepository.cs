using System.Data;
using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Models;

namespace PlayOfferService.Repositories;

public class MemberRepository
{
    private readonly DatabaseContext _context;

    public MemberRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Member> GetMemberById(Guid? memberId)
    {
        var events = await _context.Events
            .Where(e => e.EntityId == memberId)
            .OrderBy(e => e.Timestamp)
            .ToListAsync();

        if (events.Count == 0)
        {
            throw new ArgumentException("No member found with id " + memberId);
        }
        var member = new Member();
        member.Apply(events);

        return member;
    }

    public async Task UpdateEntityAsync(BaseEvent parsedEvent)
    {
        throw new NotImplementedException();
    }
}