using PlayOfferService.Domain.Events;

public class PlayOfferCreatedEvent : IDomainEvent
{
    public Guid Id { get; set; }
    public Guid Club { get; set; }
    public Guid Creator { get; set; }
    public DateTime ProposedStartTime { get; set; }
    public DateTime ProposedEndTime { get; set; }

    public PlayOfferCreatedEvent() { }

    public PlayOfferCreatedEvent(
        Guid id,
        Guid club,
        Guid creator,
        DateTime proposedStartTime,
        DateTime proposedEndTime)
    {
        Id = id;
        Club = club;
        Creator = creator;
        ProposedStartTime = proposedStartTime;
        ProposedEndTime = proposedEndTime;
    }
}