namespace PlayOfferService.Domain.Models;

public class ReservationDto
{
    public Guid Id { get; set; }
    public List<CourtDto> Courts { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsCancelled { get; set; }
    
    public ReservationDto(Reservation reservation, List<CourtDto> courts)
    {
        Id = reservation.Id;
        Courts = courts.Where(c => reservation.CourtIds.Contains(c.Id)).ToList();
        StartTime = reservation.StartTime;
        EndTime = reservation.EndTime;
        IsCancelled = reservation.IsCancelled;
    }
}