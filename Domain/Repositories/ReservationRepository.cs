using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Domain.Repositories;

public class ReservationRepository
{
    private readonly DbReadContext _context;
    
    public ReservationRepository(){}
    
    public ReservationRepository(DbReadContext context)
    {
        _context = context;
    }
    
    public virtual async Task<Reservation?> GetReservationById(Guid? reservationId)
    {
        var reservation = await _context.Reservations
            .Where(e => e.Id == reservationId)
            .ToListAsync();

        if (reservation.Count == 0)
            return null;

        return reservation.First();
    }
    
    public virtual void CreateReservation(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
    }
    
    public virtual async Task Update()
    {
        await _context.SaveChangesAsync();
    }
}