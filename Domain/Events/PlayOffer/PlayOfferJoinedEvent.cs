using PlayOfferService.Domain.Events;
using PlayOfferService.Models;

public class PlayOfferJoinedEvent : IDomainEvent
{
    public Member Opponent { get; set; }
    public DateTime AcceptedStartTime { get; set; }
    
    public PlayOfferJoinedEvent(){}
    
    public PlayOfferJoinedEvent(
        Member opponent,
        DateTime acceptedStartTime)
    {
        Opponent = opponent;
        AcceptedStartTime = acceptedStartTime;
    }
}