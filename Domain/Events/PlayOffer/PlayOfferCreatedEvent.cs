using PlayOfferService.Domain.Events;
using PlayOfferService.Models;

public class PlayOfferCreatedEvent : IDomainEvent
{
    public Guid Id { get; set; }
    public Club Club { get; set; }
    public Member Creator { get; set; }
    public DateTime ProposedStartTime { get; set; }
    public DateTime ProposedEndTime { get; set; }
    
    public PlayOfferCreatedEvent(){}

    public PlayOfferCreatedEvent(
        Guid id,
        Club club,
        Member creator,
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