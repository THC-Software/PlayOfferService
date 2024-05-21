using MediatR;
using PlayOfferService.Commands;
using PlayOfferService.Domain.Events;
using PlayOfferService.Models;
using PlayOfferService.Repositories;

namespace PlayOfferService.Handlers;
public class CreatePlayOfferHandler : IRequestHandler<CreatePlayOfferCommand, PlayOffer>
{

    private readonly DbWriteContext _context;
    private readonly ClubRepository _clubRepository;
    private readonly MemberRepository _memberRepository;
    private readonly PlayOfferRepository _playOfferRepository;

    public CreatePlayOfferHandler(DbWriteContext context, ClubRepository clubRepository, MemberRepository memberRepository, PlayOfferRepository playOfferRepository)
    {
        _context = context;
        _clubRepository = clubRepository;
        _memberRepository = memberRepository;
        _playOfferRepository = playOfferRepository;
    }

    public async Task<PlayOffer> Handle(CreatePlayOfferCommand request, CancellationToken cancellationToken)
    {
        var playOfferDto = request.playOfferDto;
        
        var creator = await _memberRepository.GetMemberById(playOfferDto.CreatorId);
        if(creator == null)
        {
            throw new ArgumentException("Creator not found");
        }
        var club = await _clubRepository.GetClubById(playOfferDto.ClubId);
        if(club == null)
        {
            throw new ArgumentException("Club not found");
        }

        var playOfferId = Guid.NewGuid();
        var playOfferCreatedEvent = new BaseEvent
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
                ProposedEndTime = playOfferDto.ProposedEndTime
            },
            Timestamp = DateTime.Now.ToUniversalTime()
        };

        _context.Events.Add(playOfferCreatedEvent);
        await _context.SaveChangesAsync();

        return (await _playOfferRepository.GetPlayOffersByIds(playOfferId)).First();
    }

}
