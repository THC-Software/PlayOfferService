using PlayOfferService.Domain.ValueObjects;

namespace PlayOfferService.Domain.Events.Member;

public class MemberFullNameChangedEvent : DomainEvent
{
    public FullName FullName { get; set; }
}