using System.Text.Json.Serialization;

namespace PlayOfferService.Domain.Events.Member;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(MemberLockedEvent), typeDiscriminator: "MEMBER_ACCOUNT_UNLOCKED")]
public class MemberUnlockedEvent : IDomainEvent
{
    
}