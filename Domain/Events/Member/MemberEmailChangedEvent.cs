namespace PlayOfferService.Domain.Events.Member;

public class MemberEmailChangedEvent : DomainEvent
{
    public string Email { get; set; }
}