using Microsoft.EntityFrameworkCore;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Repositories;

public class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {

    }

    public DbSet<PlayOffer> PlayOffers { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

}
