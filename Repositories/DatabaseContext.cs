using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Models;

namespace PlayOfferService.Repositories;

public class DatabaseContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<PlayOffer> PlayOffers { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<BaseEvent<IDomainEvent>> Events { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new BaseEventConfiguration());
    }
}