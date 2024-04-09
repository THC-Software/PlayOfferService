using Microsoft.EntityFrameworkCore;
using PlayOfferService.Models;

namespace PlayOfferService.Repositories;

public class ClubContext : DbContext
{
    public DbSet<Club> Clubs { get; set; }
    
    public ClubContext(DbContextOptions<ClubContext> options) : base(options)
    {
    }
}