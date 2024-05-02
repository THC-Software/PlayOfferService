using System.Text.Json.Serialization;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Events;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "eventType")]
[JsonDerivedType(typeof(PlayOfferCreatedEvent), typeDiscriminator: "PLAYOFFER_CREATED")]
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