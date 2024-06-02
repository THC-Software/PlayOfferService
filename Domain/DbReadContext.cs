using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Models;
using PlayOfferService.Repositories;

namespace PlayOfferService.Domain;

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
        var testClub = new Club{Id = Guid.Parse("06b812a7-5131-4510-82ff-bffac33e0f3e"), Status = Status.ACTIVE};
        var testMemberIds = new List<Guid> {Guid.Parse("40c0981d-e2f8-4af3-ae6c-17f79f3ba8c2"), Guid.Parse("ccc1c8fc-89b5-4026-b190-9d9e7e7bc18d")};
        
        var testMembers = new List<Object>{
            new {Id = testMemberIds[0], ClubId = testClub.Id, Status = Status.ACTIVE},
            new {Id = testMemberIds[1], ClubId = testClub.Id, Status = Status.ACTIVE}
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
