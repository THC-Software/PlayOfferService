using System.Text.Json.Serialization;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(ClubUnlockedEvent), typeDiscriminator: "TENNIS_CLUB_UNLOCKED")]
public class ClubUnlockedEvent : IDomainEvent
{
    
}