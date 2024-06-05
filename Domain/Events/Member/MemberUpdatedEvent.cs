using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.ValueObjects;

namespace PlayOfferService.Domain.Events.Member;

public class MemberUpdatedEvent : DomainEvent
{
    public MemberId MemberId { get; set; }
    public FullName Name { get; set; }
    public string Email { get; set; }
    public TennisClubId TennisClubId { get; set; }
    public Status Status { get; set; }
}