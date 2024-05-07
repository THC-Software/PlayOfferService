namespace PlayOfferService.Models;

public class JoinPlayOfferDto
{
    public Guid PlayOfferId { get; set; }
    public Guid OpponentId { get; set; }
    public DateTime AcceptedStartTime { get; set; }
}