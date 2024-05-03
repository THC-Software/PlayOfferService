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

        var member = new Member();
        member.Apply(events);

        return member;
    }
}