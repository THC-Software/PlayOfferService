using System.Text.Json.Serialization;
using PlayOfferService.Domain.Events;
using PlayOfferService.Models;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(PlayOfferJoinedEvent), typeDiscriminator: "PLAYOFFER_JOINED")]
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