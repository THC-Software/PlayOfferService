using MediatR;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Domain.Events.PlayOffer;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers.Events;

public class MemberEventHandler : IRequestHandler<TechnicalMemberEvent>
{
    private readonly MemberRepository _memberRepository;
    private readonly ReadEventRepository _eventRepository;
    private readonly WriteEventRepository _writeEventRepository;
    private readonly PlayOfferRepository _playOfferRepository;
    
    public MemberEventHandler(MemberRepository memberRepository, ReadEventRepository eventRepository, PlayOfferRepository playOfferRepository, WriteEventRepository writeEventRepository)
    {
        _memberRepository = memberRepository;
        _eventRepository = eventRepository;
        _playOfferRepository = playOfferRepository;
        _writeEventRepository = writeEventRepository;
    }
    
    public async Task Handle(TechnicalMemberEvent memberEvent, CancellationToken cancellationToken)
    {
        Console.WriteLine("MemberEventHandler received event: " + memberEvent.EventType);
        var existingEvent = await _eventRepository.GetEventById(memberEvent.EventId);
        if (existingEvent != null)
        {
            Console.WriteLine("Event already applied, skipping");
            return;
        }
        
        switch (memberEvent.EventType)
        {
            case EventType.MEMBER_REGISTERED:
                await HandleMemberRegisteredEvent(memberEvent);
                break;
            case EventType.MEMBER_LOCKED:
                await HandleMemberLockedEvent(memberEvent);
                break;
            case EventType.MEMBER_UNLOCKED:
                await HandleMemberUnlockedEvent(memberEvent);
                break;
            case EventType.MEMBER_DELETED:
                await HandleMemberDeletedEvent(memberEvent);
                break;
        }
        
        await _memberRepository.Update();
        await _eventRepository.AppendEvent(memberEvent);
        await _eventRepository.Update();
    }

    private async Task HandleMemberDeletedEvent(TechnicalMemberEvent memberEvent)
    {
        await CreatePlayOfferCancelledEventsByCreatorId(memberEvent);
        
        var existingMember = await _memberRepository.GetMemberById(memberEvent.EntityId);
        existingMember!.Apply([memberEvent]);
    }

    private async Task HandleMemberUnlockedEvent(TechnicalMemberEvent memberEvent)
    {
        var existingMember = await _memberRepository.GetMemberById(memberEvent.EntityId);
        existingMember!.Apply([memberEvent]);
    }

    private async Task HandleMemberLockedEvent(TechnicalMemberEvent memberEvent)
    {
        await CreatePlayOfferCancelledEventsByCreatorId(memberEvent);
        
        var existingMember = await _memberRepository.GetMemberById(memberEvent.EntityId);
        existingMember!.Apply([memberEvent]);
    }

    private async Task HandleMemberRegisteredEvent(TechnicalMemberEvent memberEvent)
    {
        var member = new Member();
        member.Apply([memberEvent]);
        _memberRepository.CreateMember(member);
    }

    private async Task CreatePlayOfferCancelledEventsByCreatorId(TechnicalMemberEvent memberEvent)
    {
        // Get all play offers by creator id
        var existingPlayOffer = await _playOfferRepository.GetPlayOffersByIds(null, memberEvent.EntityId);
        
        // Create PlayOfferCancelled events for each play offer
        foreach (var playOffer in existingPlayOffer)
        {
            var cancelledEvent = new BaseEvent
            {
                EventId = Guid.NewGuid(),
                EventType = EventType.PLAYOFFER_CANCELLED,
                EntityType = EntityType.PLAYOFFER,
                EntityId = playOffer.Id,
                Timestamp = DateTime.UtcNow,
                EventData = new PlayOfferCancelledEvent(),
                CorrelationId = memberEvent.EventId
            };
            
            await _writeEventRepository.AppendEvent(cancelledEvent);
        }
        await _writeEventRepository.Update();
    }
}