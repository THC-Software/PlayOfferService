using PlayOfferService.Models;

namespace PlayOfferService.Domain.Events.Member;

public class MemberCreatedEvent : IDomainEvent
{
    public MemberId MemberId { get; set; }
    public FullName Name { get; set; }
    public string Email { get; set; }
    public TennisClubId TennisClubId { get; set; }
    public MemberStatus Status { get; set; }
}