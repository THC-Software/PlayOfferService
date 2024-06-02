using System.ComponentModel.DataAnnotations.Schema;
using PlayOfferService.Domain.Events;

namespace PlayOfferService.Models;

public class PlayOffer {
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public Club Club { get; set; }
    public Member Creator { get; set; }
    public Member? Opponent { get; set; }
    public DateTime ProposedStartTime { get; set; }
    public DateTime ProposedEndTime { get; set; }
    public DateTime? AcceptedStartTime { get; set; }
    public Reservation? Reservation { get; set; }
    public bool IsCancelled { get; set; }

    public PlayOffer() {}

    public void Apply(List<BaseEvent> baseEvents)
    {
        foreach (var baseEvent in baseEvents)
        {
            switch (baseEvent.EventType)
            {
                case EventType.PLAYOFFER_CREATED:
                    Apply((PlayOfferCreatedEvent) baseEvent.EventData);
                    break;
                case EventType.PLAYOFFER_JOINED:
                    Apply((PlayOfferJoinedEvent) baseEvent.EventData);
                    break;
                case EventType.PLAYOFFER_CANCELLED:
                    Apply((PlayOfferCancelledEvent) baseEvent.EventData);
                    break;
                case EventType.PLAYOFFER_RESERVATION_CREATED:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(baseEvent.EventType)} is not supported for the entity Playoffer!");
            }
        }
    }
    
    private void Apply(PlayOfferCreatedEvent domainEvent)
    {
        Id = domainEvent.Id;
        Club = domainEvent.Club;
        Creator = domainEvent.Creator;
        ProposedStartTime = domainEvent.ProposedStartTime;
        ProposedEndTime = domainEvent.ProposedEndTime;
        IsCancelled = false;
    }
    
    private void Apply(PlayOfferJoinedEvent domainEvent)
    {
        AcceptedStartTime = domainEvent.AcceptedStartTime;
        Opponent = domainEvent.Opponent;
    }
    
    private void Apply(PlayOfferCancelledEvent domainEvent)
    {
        IsCancelled = true;
    }
}