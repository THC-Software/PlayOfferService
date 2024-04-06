namespace PlayOfferService.Models;

public class PlayOffer {
    public int Id { get; set; }
    public int ClubId { get; set; }
    public int CreatorId { get; set; }
    public int? OpponentId { get; set; }
    public DateTime PlayDate { get; set; }
    public int? ReservationId { get; set; }

    public PlayOffer()
    {
    }
    
    public PlayOffer(PlayOfferDto dto)
    {
        ClubId = dto.ClubId;
        CreatorId = dto.CreatorId;
        OpponentId = dto.OpponentId;
        PlayDate = dto.PlayDate;
        ReservationId = dto.ReservationId;
    }
}