using MediatR;
using PlayOfferService.Application.Queries;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers;
public class GetPlayOffersByIdHandler : IRequestHandler<GetPlayOffersByIdQuery, IEnumerable<PlayOffer>>
{
    private readonly PlayOfferRepository _playOfferRepository;

    public GetPlayOffersByIdHandler(PlayOfferRepository playOfferRepository)
    {
        _playOfferRepository = playOfferRepository;
    }

    public async Task<IEnumerable<PlayOffer>> Handle(GetPlayOffersByIdQuery request, CancellationToken cancellationToken)
    {
        return await _playOfferRepository.GetPlayOffersByIds(request.PlayOfferId, request.CreatorId, request.ClubId);
    }
}
