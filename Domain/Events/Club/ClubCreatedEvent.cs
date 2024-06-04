using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.ValueObjects;

namespace PlayOfferService.Domain.Events;

public class ClubCreatedEvent : DomainEvent
{
    public TennisClubId TennisClubId { get; set; }
    public string Name { get; set; }
    public SubscriptionTierId SubscriptionTierId { get; set; }
    public Status Status { get; set; }
}