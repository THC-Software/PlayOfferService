using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Domain.Repositories;

public class CourtRepository
{
    private readonly DbReadContext _context;
    
    public CourtRepository(){}
    
    public CourtRepository(DbReadContext context)
    {
        _context = context;
    }
    
    public async Task<List<Court>> GetAllCourts()
    {
        return await _context.Courts.ToListAsync();
    }
    
    public virtual async Task<Court?> GetCourtById(Guid? courtId)
    {
        var courts = await _context.Courts
            .Where(e => e.Id == courtId)
            .ToListAsync();

        var allCourts = await _context.Courts.ToListAsync();

        if (courts.Count == 0)
            return null;

        return courts.First();
    }
    
    public virtual void CreateCourt(Court court)
    {
        _context.Courts.Add(court);
    }
    
    public virtual async Task Update()
    {
        await _context.SaveChangesAsync();
    }
}