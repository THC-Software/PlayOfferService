namespace PlayOfferService.Models;

public class PlayOfferDto
{
    public Guid ClubId { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime ProposedStartTime { get; set; }
    public DateTime ProposedEndTime { get; set; }
}