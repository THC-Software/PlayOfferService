using System.Text.Json.Serialization;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Events;

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