using System.ComponentModel.DataAnnotations.Schema;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.PlayOffer;

namespace PlayOfferService.Domain.Models;

public class PlayOffer
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public Guid CreatorId { get; set; }
    public Guid? OpponentId { get; set; }
    public DateTime ProposedStartTime { get; set; }
    public DateTime ProposedEndTime { get; set; }
    public DateTime? AcceptedStartTime { get; set; }
    public Guid? ReservationId { get; set; }
    public bool IsCancelled { get; set; }

    public PlayOffer() { }

    public void Apply(List<BaseEvent> baseEvents)
    {
        foreach (var baseEvent in baseEvents)
        {
            switch (baseEvent.EventType)
            {
                case EventType.PLAYOFFER_CREATED:
                    Apply((PlayOfferCreatedEvent)baseEvent.EventData);
                    break;
                case EventType.PLAYOFFER_JOINED:
                    Apply((PlayOfferJoinedEvent)baseEvent.EventData);
                    break;
                case EventType.PLAYOFFER_CANCELLED:
                    Apply((PlayOfferCancelledEvent)baseEvent.EventData);
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
        ClubId = domainEvent.ClubId;
        CreatorId = domainEvent.CreatorId;
        ProposedStartTime = domainEvent.ProposedStartTime;
        ProposedEndTime = domainEvent.ProposedEndTime;
        IsCancelled = false;
    }

    private void Apply(PlayOfferJoinedEvent domainEvent)
    {
        AcceptedStartTime = domainEvent.AcceptedStartTime;
        OpponentId = domainEvent.OpponentId;
    }

    private void Apply(PlayOfferCancelledEvent domainEvent)
    {
        IsCancelled = true;
    }
}