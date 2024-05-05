namespace PlayOfferService.Domain.Events;

public class ClubCreatedEvent : IDomainEvent
{
    public Guid TennisClubId { get; set; }
}