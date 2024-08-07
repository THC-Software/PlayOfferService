﻿using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Models;
using PlayOfferService.Migrations.EntityConfiguration;

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
    public DbSet<Court> Courts { get; set; }
    public DbSet<BaseEvent> AppliedEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new BaseEventConfiguration());

        // TODO: Remove before coop testing
        /*
        var testClub = new Club { Id = Guid.Parse("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"), Name = "Test Club", Status = Status.ACTIVE };
        var testMemberIds = new List<Guid> { Guid.Parse("0033506d-2d59-40d3-b996-74a251091027"), Guid.Parse("ccc1c8fc-89b5-4026-b190-9d9e7e7bc18d") };

        var testMembers = new List<Object>{
            new {Id = testMemberIds[0], ClubId = testClub.Id, Status = Status.ACTIVE, FirstName = "Hans", LastName="Müller", Email="hans@müller.de"},
            new {Id = testMemberIds[1], ClubId = testClub.Id, Status = Status.ACTIVE, FirstName = "Friedrich", LastName="Bäcker", Email="friedrich@bäcker.at"}
        };

        // Need to directly specify foreign keys for seeding
        var testPlayOffer = new
        {
            Id = Guid.NewGuid(),
            ClubId = testClub.Id,
            CreatorId = testMemberIds[1],
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
        */
    }
}
