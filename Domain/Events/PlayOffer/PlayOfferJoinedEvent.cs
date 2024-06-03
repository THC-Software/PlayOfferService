namespace PlayOfferService.Domain.Events.PlayOffer;

public class PlayOfferJoinedEvent : DomainEvent
{
    public Guid OpponentId { get; set; }
    public DateTime AcceptedStartTime { get; set; }
    
    public PlayOfferJoinedEvent(){}
}