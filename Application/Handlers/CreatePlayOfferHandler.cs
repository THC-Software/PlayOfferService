using MediatR;
using PlayOfferService.Application.Commands;
using PlayOfferService.Application.Exceptions;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers;
public class CreatePlayOfferHandler : IRequestHandler<CreatePlayOfferCommand, Guid>
{

    private readonly WriteEventRepository _writeEventRepository;
    private readonly ClubRepository _clubRepository;
    private readonly MemberRepository _memberRepository;

    public CreatePlayOfferHandler(WriteEventRepository writeEventRepository, ClubRepository clubRepository, MemberRepository memberRepository)
    {
        _writeEventRepository = writeEventRepository;
        _clubRepository = clubRepository;
        _memberRepository = memberRepository;
    }

    public async Task<Guid> Handle(CreatePlayOfferCommand request, CancellationToken cancellationToken)
    {
        var playOfferDto = request.CreatePlayOfferDto;
        
        var club = await _clubRepository.GetClubById(request.ClubId);
        if(club == null)
            throw new NotFoundException($"Club {request.ClubId} not found");
        switch (club.Status)
        {
            case Status.LOCKED:
                throw new InvalidOperationException("Can't create PlayOffer while club is locked!");
            case Status.DELETED:
                throw new InvalidOperationException("Can't create PlayOffer in deleted club!");
        }
        
        var creator = await _memberRepository.GetMemberById(request.CreatorId);
        if(creator == null)
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

        await _writeEventRepository.AppendEvent(domainEvent);
        await _writeEventRepository.Update();

        return playOfferId;
    }

}
