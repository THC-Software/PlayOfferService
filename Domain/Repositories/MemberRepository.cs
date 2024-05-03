using System.Data;
using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
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

        if (events.First().EventType != EventType.MEMBER_ACCOUNT_CREATED)
        {
            throw new DataException("INVALID STATE: First Member Event must be of type " +
                                    nameof(EventType.MEMBER_ACCOUNT_CREATED));
        }

        var member = new Member();
        foreach (var baseEvent in events)
        {
            member.Apply(baseEvent);
        }

        return member;
    }
}