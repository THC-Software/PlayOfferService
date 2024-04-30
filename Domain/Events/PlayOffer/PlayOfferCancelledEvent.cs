using System.Text.Json.Serialization;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(PlayOfferCancelledEvent), typeDiscriminator: "PLAYOFFER_CANCELLED")]
public class PlayOfferCancelledEvent : IDomainEvent
{
}