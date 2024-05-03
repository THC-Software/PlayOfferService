using System.Text.Json.Serialization;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Events.Member;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(MemberCreatedEvent), typeDiscriminator: "MEMBER_ACCOUNT_CREATED")]
public class MemberCreatedEvent : IDomainEvent
{
    public Guid MemberAccountId { get; set; }
    public Club Club { get; set; }
}