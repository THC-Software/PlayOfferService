using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Events;

public class ClubCreatedEvent : IDomainEvent
{
    public TennisClubId TennisClubId { get; set; }
    public string Name { get; set; }
    public SubscriptionTierId SubscriptionTierId { get; set; }
    public Status Status { get; set; }
}