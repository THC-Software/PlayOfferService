using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Models;

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
    
    public virtual void CreateMember(Member member)
    {
        _context.Members.Add(member);
    }

    public virtual async Task Update()
    {
        await _context.SaveChangesAsync();
    }
}