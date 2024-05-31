using PlayOfferService.Domain.Events;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayOfferService.Models;

public class PlayOffer
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public Guid Club { get; set; }
    public Guid Creator { get; set; }
    public Member? Opponent { get; set; }
    public DateTime ProposedStartTime { get; set; }
    public DateTime ProposedEndTime { get; set; }
    public DateTime? AcceptedStartTime { get; set; }
    public Reservation? Reservation { get; set; }
    public bool IsCancelled { get; set; }

    public PlayOffer() { }

    public void Apply(List<BaseEvent> baseEvents)
    {
        if (Id == Guid.Empty && baseEvents.First().EventType != EventType.PLAYOFFER_CREATED)
        {
            throw new ArgumentException("First PlayOffer event must be of type "
                                        + nameof(EventType.PLAYOFFER_CREATED));
        }

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
                    throw new ArgumentOutOfRangeException();
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
        if (IsCancelled)
            throw new ArgumentException("Can't join cancelled PlayOffer");

        if (domainEvent.AcceptedStartTime < ProposedStartTime || domainEvent.AcceptedStartTime > ProposedEndTime)
            throw new ArgumentException("Accepted start time must be within the proposed start and end time");

        if (domainEvent.Opponent.Id == Creator)
            throw new ArgumentException("Creator can't join his own PlayOffer");

        if (domainEvent.Opponent.ClubId != Club)
            throw new ArgumentException("Opponent must be from the same club as the creator of the PlayOffer");

        AcceptedStartTime = domainEvent.AcceptedStartTime;
        Opponent = domainEvent.Opponent;
    }

    private void Apply(PlayOfferCancelledEvent domainEvent)
    {
        IsCancelled = true;
    }
}