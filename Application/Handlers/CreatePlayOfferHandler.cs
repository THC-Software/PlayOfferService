using MediatR;
using PlayOfferService.Application.Commands;
using PlayOfferService.Application.Exceptions;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers;

public class CreatePlayOfferHandler : IRequestHandler<CreatePlayOfferCommand, Guid>
{
    private readonly ClubRepository _clubRepository;
    private readonly MemberRepository _memberRepository;

    private readonly WriteEventRepository _writeEventRepository;

    public CreatePlayOfferHandler(WriteEventRepository writeEventRepository, ClubRepository clubRepository,
        MemberRepository memberRepository)
    {
        _writeEventRepository = writeEventRepository;
        _clubRepository = clubRepository;
        _memberRepository = memberRepository;
    }

    public async Task<Guid> Handle(CreatePlayOfferCommand request, CancellationToken cancellationToken)
    {
        var playOfferDto = request.CreatePlayOfferDto;
        
        if (playOfferDto.ProposedStartTime >= playOfferDto.ProposedEndTime)
            throw new InvalidOperationException("Proposed start time must be before proposed end time!");

        var club = await _clubRepository.GetClubById(request.ClubId);
        if (club == null)
            throw new NotFoundException($"Club {request.ClubId} not found");
        switch (club.Status)
        {
            case Status.LOCKED:
                throw new InvalidOperationException("Can't create PlayOffer while club is locked!");
            case Status.DELETED:
                throw new InvalidOperationException("Can't create PlayOffer in deleted club!");
        }

        var creator = await _memberRepository.GetMemberById(request.CreatorId);
        if (creator == null)
            throw new NotFoundException($"Member {request.CreatorId} not found!");
        switch (creator.Status)
        {
            case Status.LOCKED:
                throw new InvalidOperationException("Can't create PlayOffer while member is locked!");
            case Status.DELETED:
                throw new InvalidOperationException("Can't create PlayOffer as a deleted member!");
        }

        var playOfferId = Guid.NewGuid();
        var domainEvent = new BaseEvent
        {
            EntityId = playOfferId,
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.NewGuid(),
            EventType = EventType.PLAYOFFER_CREATED,
            EventData = new PlayOfferCreatedEvent
            {
                Id = playOfferId,
                ClubId = club.Id,
                CreatorId = creator.Id,
                ProposedStartTime = playOfferDto.ProposedStartTime.ToUniversalTime(),
                ProposedEndTime = playOfferDto.ProposedEndTime.ToUniversalTime()
            },
            Timestamp = DateTime.Now.ToUniversalTime()
        };

        var transaction = _writeEventRepository.StartTransaction();
        var excpectedEventCount = _writeEventRepository.GetEventCount(playOfferId) + 1;

        await _writeEventRepository.AppendEvent(domainEvent);
        await _writeEventRepository.Update();

        var eventCount = _writeEventRepository.GetEventCount(playOfferId);

        if (eventCount != excpectedEventCount)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Concurrent modification detected!");
        }

        transaction.Commit();

        return playOfferId;
    }
}