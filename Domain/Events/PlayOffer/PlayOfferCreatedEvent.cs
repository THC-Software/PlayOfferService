using PlayOfferService.Domain.Events;

public class PlayOfferCreatedEvent : DomainEvent
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime ProposedStartTime { get; set; }
    public DateTime ProposedEndTime { get; set; }

    public PlayOfferCreatedEvent() { }

    public PlayOfferCreatedEvent(
        Guid id,
        Guid clubId,
        Guid creatorId,
        DateTime proposedStartTime,
        DateTime proposedEndTime)
    {
        Id = id;
        ClubId = clubId;
        CreatorId = creatorId;
        ProposedStartTime = proposedStartTime;
        ProposedEndTime = proposedEndTime;
    }
}