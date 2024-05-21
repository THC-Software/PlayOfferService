using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Models;

namespace PlayOfferService.Repositories;

public class DbWriteContext : DbContext
{
    public DbWriteContext(DbContextOptions<DbWriteContext> options) : base(options)
    {

    }
    
    public DbSet<BaseEvent> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new BaseEventConfiguration());
    }
}