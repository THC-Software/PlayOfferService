using System.Text.Json.Serialization;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(ClubLockedEvent), typeDiscriminator: "TENNIS_CLUB_LOCKED")]
public class ClubLockedEvent : IDomainEvent
{
    
}