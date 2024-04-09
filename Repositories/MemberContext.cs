using Microsoft.EntityFrameworkCore;
using PlayOfferService.Models;

namespace PlayOfferService.Repositories;

public class MemberContext : DbContext
{
    public DbSet<Member> Members { get; set; }
    
    public MemberContext(DbContextOptions<MemberContext> options) : base(options)
    {
    }
}