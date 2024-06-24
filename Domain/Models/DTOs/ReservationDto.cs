namespace PlayOfferService.Domain.Models;

public class ReservationDto
{
    public Guid Id { get; set; }
    public CourtDto Court { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsCancelled { get; set; }
    
    public ReservationDto(Reservation reservation, CourtDto court)
    {
        Id = reservation.Id;
        Court = court;
        StartTime = reservation.StartTime;
        EndTime = reservation.EndTime;
        IsCancelled = reservation.IsCancelled;
    }
}