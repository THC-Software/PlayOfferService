using System.Text.Json.Serialization;
using PlayOfferService.Domain.Events.Member;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(ClubCreatedEvent), typeDiscriminator: "TENNIS_CLUB_REGISTERED")]
[JsonDerivedType(typeof(ClubLockedEvent), typeDiscriminator: "TENNIS_CLUB_LOCKED")]
[JsonDerivedType(typeof(ClubUnlockedEvent), typeDiscriminator: "TENNIS_CLUB_UNLOCKED")]
[JsonDerivedType(typeof(MemberCreatedEvent), typeDiscriminator: "MEMBER_ACCOUNT_CREATED")]
[JsonDerivedType(typeof(MemberLockedEvent), typeDiscriminator: "MEMBER_ACCOUNT_LOCKED")]
[JsonDerivedType(typeof(MemberUnlockedEvent), typeDiscriminator: "MEMBER_ACCOUNT_UNLOCKED")]
[JsonDerivedType(typeof(PlayOfferCreatedEvent), typeDiscriminator: "PLAYOFFER_CREATED")]
[JsonDerivedType(typeof(PlayOfferJoinedEvent), typeDiscriminator: "PLAYOFFER_JOINED")]
[JsonDerivedType(typeof(PlayOfferCancelledEvent), typeDiscriminator: "PLAYOFFER_CANCELLED")]
[JsonDerivedType(typeof(PlayOfferReservationCreatedEvent), typeDiscriminator: "PLAYOFFER_RESERVATION_CREATED")]
public class IDomainEvent
{
    
}