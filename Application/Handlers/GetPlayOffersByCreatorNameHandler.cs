using MediatR;
using PlayOfferService.Application.Queries;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers.Events;

public class GetPlayOffersByCreatorNameHandler : IRequestHandler<GetPlayOffersByCreatorNameQuery, IEnumerable<PlayOffer>>
{
    private readonly PlayOfferRepository _playOfferRepository;
    private readonly MemberRepository _memberRepository;
    
    public GetPlayOffersByCreatorNameHandler(PlayOfferRepository playOfferRepository, MemberRepository memberRepository)
    {
        _playOfferRepository = playOfferRepository;
        _memberRepository = memberRepository;
    }
    
    public async Task<IEnumerable<PlayOffer>> Handle(GetPlayOffersByCreatorNameQuery request, CancellationToken cancellationToken)
    {
        if (request.CreatorName.Split(" ").Length > 2)
            throw new ArgumentException("Creator name must be in the format '<FirstName> <LastName>', '<FirstName>' or '<LastName>'");
        
        var creators = await _memberRepository.GetMemberByName(request.CreatorName);
        var playOffers = new List<PlayOffer>();
        foreach (var creator in creators)
        {
            var playOffersByCreator = await _playOfferRepository.GetPlayOffersByIds(null, creator.Id);
            playOffers.AddRange(playOffersByCreator);
        }
        
        return playOffers;
    }
}