using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Repositories;

namespace PlayOfferService.Domain;

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