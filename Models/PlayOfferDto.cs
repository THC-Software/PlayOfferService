namespace PlayOfferService.Models;

public class PlayOfferDto
{
    public int ClubId { get; set; }
    public int CreatorId { get; set; }
    public DateTime ProposedStartTime { get; set; }
    public DateTime ProposedEndTime { get; set; }
}