using MediatR;
using PlayOfferService.Application.Exceptions;
using PlayOfferService.Commands;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Repositories;
using PlayOfferService.Models;

namespace PlayOfferService.Handlers;
public class CreatePlayOfferHandler : IRequestHandler<CreatePlayOfferCommand, Guid>
{

    private readonly DbWriteContext _context;
    private readonly ClubRepository _clubRepository;
    private readonly MemberRepository _memberRepository;

    public CreatePlayOfferHandler(DbWriteContext context, ClubRepository clubRepository, MemberRepository memberRepository)
    {
        _context = context;
        _clubRepository = clubRepository;
        _memberRepository = memberRepository;
    }

    public async Task<Guid> Handle(CreatePlayOfferCommand request, CancellationToken cancellationToken)
    {
        var playOfferDto = request.playOfferDto;
        
        var creator = await _memberRepository.GetMemberById(playOfferDto.CreatorId);
        if(creator == null)
            throw new NotFoundException($"Member {request.playOfferDto.CreatorId} not found!");
        
        var club = await _clubRepository.GetClubById(playOfferDto.ClubId);
        if(club == null)
            throw new ArgumentException($"Club {request.playOfferDto.ClubId} not found");

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
                Club = club,
                Creator = creator,
                ProposedStartTime = playOfferDto.ProposedStartTime.ToUniversalTime(),
                ProposedEndTime = playOfferDto.ProposedEndTime.ToUniversalTime()
            },
            Timestamp = DateTime.Now.ToUniversalTime()
        };

        _context.Events.Add(domainEvent);
        await _context.SaveChangesAsync();

        return playOfferId;
    }

}
