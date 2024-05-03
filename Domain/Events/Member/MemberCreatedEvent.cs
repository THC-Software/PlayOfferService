using PlayOfferService.Models;

namespace PlayOfferService.Domain.Events.Member;

public class MemberCreatedEvent : IDomainEvent
{
    public Guid MemberAccountId { get; set; }
    public Club Club { get; set; }
}