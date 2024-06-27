namespace PlayOfferService.Domain.Events.Club;

public class ClubNameChangedEvent : DomainEvent
{
    public string Name { get; set; }
}
