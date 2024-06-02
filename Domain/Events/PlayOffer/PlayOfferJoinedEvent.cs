using PlayOfferService.Domain.Events;
using PlayOfferService.Models;

public class PlayOfferJoinedEvent : IDomainEvent
{
    public Guid OpponentId { get; set; }
    public DateTime AcceptedStartTime { get; set; }
    
    public PlayOfferJoinedEvent(){}
}