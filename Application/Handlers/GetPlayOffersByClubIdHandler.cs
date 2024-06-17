using MediatR;
using PlayOfferService.Application.Queries;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers;
public class GetPlayOffersByClubIdHandler : IRequestHandler<GetPlayOffersByClubIdQuery, IEnumerable<PlayOffer>>
{
    private readonly PlayOfferRepository _playOfferRepository;

    public GetPlayOffersByClubIdHandler(PlayOfferRepository playOfferRepository)
    {
        _playOfferRepository = playOfferRepository;
    }

    public async Task<IEnumerable<PlayOffer>> Handle(GetPlayOffersByClubIdQuery request, CancellationToken cancellationToken)
    {
        return await _playOfferRepository.GetPlayOffersByIds(null, null, request.ClubId);
    }
}
