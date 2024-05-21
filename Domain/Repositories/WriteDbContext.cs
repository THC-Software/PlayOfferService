using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Models;
using PlayOfferService.Repositories;

namespace PlayOfferService.Domain.Repositories;

public class WriteDbContext : DbContext
{

    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
    {

    }
    public DbSet<BaseEvent> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new BaseEventConfiguration());

        // TODO: Remove before coop testing
        var testClubId = Guid.NewGuid();
        var testMemberIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        modelBuilder.Entity<BaseEvent>().HasData(
            new BaseEvent
            {
                EventId = Guid.NewGuid(),
                EventType = EventType.TENNIS_CLUB_REGISTERED,
                EntityId = testClubId,
                EntityType = EntityType.CLUB,
                EventData = new ClubCreatedEvent
                {
                    TennisClubId = testClubId,
                },
                Timestamp = DateTime.UtcNow
            });

        foreach (var testMemberId in testMemberIds)
        {
            modelBuilder.Entity<BaseEvent>().HasData(
                new BaseEvent
                {
                    EntityId = testMemberId,
                    EntityType = EntityType.MEMBER,
                    EventId = Guid.NewGuid(),
                    EventType = EventType.MEMBER_ACCOUNT_CREATED,
                    EventData = new MemberCreatedEvent
                    {
                        MemberAccountId = testMemberId,
                        Club = new Club
                        {
                            Id = testClubId
                        }
                    },
                    Timestamp = DateTime.UtcNow
                });
        }
    }
}
