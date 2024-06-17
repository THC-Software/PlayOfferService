namespace PlayOfferService.Domain.Models;

public class PlayOfferDto
{
    public Guid Id { get; set; }
    public ClubDto Club { get; set; }
    public MemberDto Creator { get; set; }
    public MemberDto? Opponent { get; set; }
    public DateTime ProposedStartTime { get; set; }
    public DateTime ProposedEndTime { get; set; }
    public DateTime? AcceptedStartTime { get; set; }
    public ReservationDto? Reservation { get; set; }
    public bool IsCancelled { get; set; }
    
    public PlayOfferDto(PlayOffer playOffer, ClubDto club, MemberDto creator, MemberDto? opponent, ReservationDto? reservation)
    {
        Id = playOffer.Id;
        Club = club;
        Creator = creator;
        Opponent = opponent;
        ProposedStartTime = playOffer.ProposedStartTime;
        ProposedEndTime = playOffer.ProposedEndTime;
        AcceptedStartTime = playOffer.AcceptedStartTime;
        Reservation = reservation;
        IsCancelled = playOffer.IsCancelled;
    }
}