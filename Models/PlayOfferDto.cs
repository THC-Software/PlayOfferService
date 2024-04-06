namespace PlayOfferService.Models;

public class PlayOfferDto
{
    public int ClubId { get; set; }
    public int CreatorId { get; set; }
    public int? OpponentId { get; set; }
    public DateTime PlayDate { get; set; }
    public int? ReservationId { get; set; }
}