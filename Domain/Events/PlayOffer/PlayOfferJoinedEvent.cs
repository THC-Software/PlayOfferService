using System.Text.Json.Serialization;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(PlayOfferJoinedEvent), typeDiscriminator: "PLAYOFFER_JOINED")]
public class PlayOfferJoinedEvent(
    Member opponent, 
    DateTime acceptedStartTime)
    : IDomainEvent
{
    public Member Opponent { get; set; } = opponent;
    public DateTime AcceptedStartTime { get; set; } = acceptedStartTime;
}