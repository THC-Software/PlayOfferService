using System.Text.Json.Serialization;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(ClubCreatedEvent), typeDiscriminator: "TENNIS_CLUB_REGISTERED")]
public class ClubCreatedEvent : IDomainEvent
{
    public Guid TennisClubId { get; set; }
}