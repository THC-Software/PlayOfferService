namespace PlayOfferService.Domain.Models;

public class JoinPlayOfferDto
{
    public Guid PlayOfferId { get; set; }
    public DateTime AcceptedStartTime { get; set; }
}