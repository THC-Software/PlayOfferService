using Microsoft.EntityFrameworkCore;
using PlayOfferService.Models;

namespace PlayOfferService.Repositories;

public class ReservationContext : DbContext
{
    public DbSet<Reservation> Reservations { get; set; }
    
    public ReservationContext(DbContextOptions<ReservationContext> options) : base(options)
    {
    }
}