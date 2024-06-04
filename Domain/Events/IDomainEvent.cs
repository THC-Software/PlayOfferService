using System.Text.Json.Serialization;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Domain.Events.PlayOffer;
using PlayOfferService.Domain.Events.Reservation;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(ClubCreatedEvent), typeDiscriminator: "TENNIS_CLUB_REGISTERED")]
[JsonDerivedType(typeof(ClubLockedEvent), typeDiscriminator: "TENNIS_CLUB_LOCKED")]
[JsonDerivedType(typeof(ClubUnlockedEvent), typeDiscriminator: "TENNIS_CLUB_UNLOCKED")]
[JsonDerivedType(typeof(ClubDeletedEvent), typeDiscriminator: "TENNIS_CLUB_DELETED")]
[JsonDerivedType(typeof(MemberCreatedEvent), typeDiscriminator: "MEMBER_REGISTERED")]
[JsonDerivedType(typeof(MemberLockedEvent), typeDiscriminator: "MEMBER_LOCKED")]
[JsonDerivedType(typeof(MemberUnlockedEvent), typeDiscriminator: "MEMBER_UNLOCKED")]
[JsonDerivedType(typeof(MemberDeletedEvent), typeDiscriminator: "MEMBER_DELETED")]
[JsonDerivedType(typeof(PlayOfferCreatedEvent), typeDiscriminator: "PLAYOFFER_CREATED")]
[JsonDerivedType(typeof(PlayOfferJoinedEvent), typeDiscriminator: "PLAYOFFER_JOINED")]
[JsonDerivedType(typeof(PlayOfferCancelledEvent), typeDiscriminator: "PLAYOFFER_CANCELLED")]
[JsonDerivedType(typeof(ReservationCreatedEvent), typeDiscriminator: "ReservationCreatedEvent")]
public class DomainEvent
{
}