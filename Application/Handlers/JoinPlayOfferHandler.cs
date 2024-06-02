using MediatR;
using PlayOfferService.Application.Exceptions;
using PlayOfferService.Commands;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Repositories;
using PlayOfferService.Models;

namespace PlayOfferService.Application.Handlers;

public class JoinPlayOfferHandler : IRequestHandler<JoinPlayOfferCommand, Task>
{
    private readonly DbWriteContext _context;
    private readonly PlayOfferRepository _playOfferRepository;
    private readonly MemberRepository _memberRepository;

    public JoinPlayOfferHandler(DbWriteContext context, PlayOfferRepository playOfferRepository, MemberRepository memberRepository)
    {
        _context = context;
        _playOfferRepository = playOfferRepository;
        _memberRepository = memberRepository;
    }

    public async Task<Task> Handle(JoinPlayOfferCommand request, CancellationToken cancellationToken)
    {
        var existingPlayOffer = (await _playOfferRepository.GetPlayOffersByIds(request.joinPlayOfferDto.PlayOfferId)).FirstOrDefault();
        if (existingPlayOffer == null)
            throw new NotFoundException($"PlayOffer {request.joinPlayOfferDto.PlayOfferId} not found!");
        
        var existingOpponent = await _memberRepository.GetMemberById(request.joinPlayOfferDto.OpponentId);
        if (existingOpponent == null)
            throw new NotFoundException($"Member {request.joinPlayOfferDto.OpponentId} not found!");
        
        if (existingOpponent.Id == existingPlayOffer.Creator.Id)
            throw new InvalidOperationException("Can't join your own PlayOffer!");
        
        if (existingPlayOffer.IsCancelled)
            throw new InvalidOperationException("Can't join cancelled PlayOffer!");
        
        if (request.joinPlayOfferDto.AcceptedStartTime < existingPlayOffer.ProposedStartTime ||
            request.joinPlayOfferDto.AcceptedStartTime > existingPlayOffer.ProposedEndTime)
            throw new InvalidOperationException("Accepted start time must be within the proposed start and end time");
        
        if (existingOpponent.ClubId != existingPlayOffer.Creator.ClubId)
            throw new InvalidOperationException("Opponent must be from the same club as the creator of the PlayOffer");
        
        switch (existingPlayOffer.Club.Status)
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
            EntityId = request.joinPlayOfferDto.PlayOfferId,
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.NewGuid(),
            EventType = EventType.PLAYOFFER_JOINED,
            EventData = new PlayOfferJoinedEvent
            {
                Opponent = existingOpponent,
                AcceptedStartTime = request.joinPlayOfferDto.AcceptedStartTime.ToUniversalTime(),
            },
            Timestamp = DateTime.UtcNow
        };

        _context.Events.Add(domainEvent);
        await _context.SaveChangesAsync();

        return Task.CompletedTask;
    }
}
