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
    
    public virtual async Task<List<Member>> GetAllMembers()
    {
        return await _context.Members.ToListAsync();
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
    
    public virtual async Task<List<Member>> GetMemberByName(string creatorName)
    {
        var firstAndLastName = creatorName.Split(" ");

        var members = new List<Member>();
        if (firstAndLastName.Length == 2)
        {
            members = await _context.Members
                .Where(e => e.FirstName.ToLower().Contains(firstAndLastName[0].ToLower()) && e.LastName.ToLower().Contains(firstAndLastName[1].ToLower()))
                .ToListAsync();
        } else if (firstAndLastName.Length == 1)
        {
            members = await _context.Members
                .Where(e => e.FirstName.ToLower().Contains(firstAndLastName[0].ToLower()) || e.LastName.ToLower().Contains(firstAndLastName[0].ToLower()))
                .ToListAsync();
        }
        
        return members;
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