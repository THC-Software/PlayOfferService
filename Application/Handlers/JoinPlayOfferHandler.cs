using MediatR;
using PlayOfferService.Application.Commands;
using PlayOfferService.Application.Exceptions;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.PlayOffer;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers;

public class JoinPlayOfferHandler : IRequestHandler<JoinPlayOfferCommand, Task>
{
    private readonly WriteEventRepository _writeEventRepository;
    private readonly PlayOfferRepository _playOfferRepository;
    private readonly MemberRepository _memberRepository;
    private readonly ClubRepository _clubRepository;

    public JoinPlayOfferHandler(WriteEventRepository writeEventRepository, PlayOfferRepository playOfferRepository, MemberRepository memberRepository, ClubRepository clubRepository)
    {
        _writeEventRepository = writeEventRepository;
        _playOfferRepository = playOfferRepository;
        _memberRepository = memberRepository;
        _clubRepository = clubRepository;
    }

    public async Task<Task> Handle(JoinPlayOfferCommand request, CancellationToken cancellationToken)
    {
        var existingPlayOffer = (await _playOfferRepository.GetPlayOffersByIds(request.JoinPlayOfferDto.PlayOfferId)).FirstOrDefault();
        if (existingPlayOffer == null)
            throw new NotFoundException($"PlayOffer {request.JoinPlayOfferDto.PlayOfferId} not found!");
        
        var existingOpponent = await _memberRepository.GetMemberById(request.MemberId);
        if (existingOpponent == null)
            throw new NotFoundException($"Member {request.MemberId} not found!");
        
        if (existingOpponent.Id == existingPlayOffer.CreatorId)
            throw new InvalidOperationException("Can't join your own PlayOffer!");
        
        if (existingPlayOffer.IsCancelled)
            throw new InvalidOperationException("Can't join cancelled PlayOffer!");
        
        if (request.JoinPlayOfferDto.AcceptedStartTime < existingPlayOffer.ProposedStartTime ||
            request.JoinPlayOfferDto.AcceptedStartTime > existingPlayOffer.ProposedEndTime)
            throw new InvalidOperationException("Accepted start time must be within the proposed start and end time");
        
        var existingCreator = await _memberRepository.GetMemberById(existingPlayOffer.CreatorId);
        if (existingOpponent.ClubId != existingCreator!.ClubId)
            throw new InvalidOperationException("Opponent must be from the same club as the creator of the PlayOffer");
        
        var existingClub = await _clubRepository.GetClubById(existingPlayOffer.ClubId);
        switch (existingClub!.Status)
        {
            case Status.LOCKED:
                throw new InvalidOperationException("Can't join PlayOffer while club is locked!");
            case Status.DELETED:
                throw new InvalidOperationException("Can't join PlayOffer in deleted club!");
        }
        
        switch (existingOpponent.Status)
        {
            case Status.LOCKED:
                throw new InvalidOperationException("Can't join PlayOffer while member is locked!");
            case Status.DELETED:
                throw new InvalidOperationException("Can't join PlayOffer as a deleted member!");
        }
        
        var domainEvent = new BaseEvent
        {
            EntityId = request.JoinPlayOfferDto.PlayOfferId,
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.NewGuid(),
            EventType = EventType.PLAYOFFER_JOINED,
            EventData = new PlayOfferJoinedEvent
            {
                OpponentId = existingOpponent.Id,
                AcceptedStartTime = request.JoinPlayOfferDto.AcceptedStartTime.ToUniversalTime(),
            },
            Timestamp = DateTime.UtcNow
        };

        await _writeEventRepository.AppendEvent(domainEvent);
        await _writeEventRepository.Update();

        return Task.CompletedTask;
    }
}
