using MediatR;
using PlayOfferService.Application.Queries;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers;
public class GetPlayOffersByParticipantIdHandler : IRequestHandler<GetPlayOffersByParticipantIdQuery, IEnumerable<PlayOffer>>
{
    private readonly PlayOfferRepository _playOfferRepository;

    public GetPlayOffersByParticipantIdHandler(PlayOfferRepository playOfferRepository)
    {
        _playOfferRepository = playOfferRepository;
    }

    public async Task<IEnumerable<PlayOffer>> Handle(GetPlayOffersByParticipantIdQuery request, CancellationToken cancellationToken)
    {
        return await _playOfferRepository.GetPlayOffersByParticipantId(request.ParticipantId);
    }
}
