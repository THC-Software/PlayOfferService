using System.ComponentModel.DataAnnotations.Schema;
using PlayOfferService.Domain.Events;

namespace PlayOfferService.Models;

public class PlayOffer {
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public Club Club { get; set; }
    public Member Creator { get; set; }
    public Member? Opponent { get; set; }
    public DateTime ProposedStartTime { get; set; }
    public DateTime ProposedEndTime { get; set; }
    public DateTime AcceptedStartTime { get; set; }
    public Reservation? Reservation { get; set; }

    public PlayOffer()
    {
    }
    
    public PlayOffer(PlayOfferDto dto)
    {
        Id = new Guid();
        Club = new Club { Id= dto.ClubId };
        Creator = new Member { Id = dto.CreatorId };
        ProposedStartTime = dto.ProposedStartTime;
        ProposedEndTime = dto.ProposedEndTime;
    }
}