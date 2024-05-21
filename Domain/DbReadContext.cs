using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Models;
using PlayOfferService.Repositories;

namespace PlayOfferService.Domain.Repositories;

public class DbReadContext : DbContext
{
    public DbReadContext(DbContextOptions<DbReadContext> options) : base(options)
    {

    }
    
    public DbSet<PlayOffer> PlayOffers { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    
    public DbSet<BaseEvent> AppliedEvents { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new BaseEventConfiguration());
        
        // TODO: Remove before coop testing
        var testClub = new Club{Id = Guid.NewGuid(), IsLocked = false};
        var testMemberIds = new List<Guid> {Guid.NewGuid(), Guid.NewGuid()};
        
        var testMembers = new List<Object>{
            new {Id = testMemberIds[0], ClubId = testClub.Id, IsLocked = false},
            new {Id = testMemberIds[1], ClubId = testClub.Id, IsLocked = false}
        };
        
        // Need to directly specify foreign keys for seeding
        var testPlayOffer = new
        {
            Id = Guid.NewGuid(),
            ClubId = testClub.Id,
            CreatorId = testMemberIds[0],
            ProposedStartTime = DateTime.UtcNow,
            ProposedEndTime = DateTime.UtcNow.AddHours(1),
            IsCancelled = false
        };
        
        modelBuilder.Entity<Club>().HasData(testClub);
        foreach (var testMember in testMembers)
        {
            modelBuilder.Entity<Member>().HasData(testMember);
        }
        modelBuilder.Entity<PlayOffer>().HasData(testPlayOffer);
    }
}
