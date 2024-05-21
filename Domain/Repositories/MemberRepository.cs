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

    public async Task UpdateEntityAsync(BaseEvent parsedEvent)
    {
        throw new NotImplementedException();
    }
}