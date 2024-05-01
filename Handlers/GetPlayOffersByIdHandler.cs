using MediatR;
using Microsoft.EntityFrameworkCore;
using PlayOfferService.Models;
using PlayOfferService.Queries;
using PlayOfferService.Repositories;

namespace PlayOfferService.Handlers;
public class GetPlayOffersByIdHandler : IRequestHandler<GetPlayOffersByIdQuery, IEnumerable<PlayOffer>> {
    private readonly DatabaseContext _context;

    public GetPlayOffersByIdHandler(DatabaseContext context) {
        _context = context;
    }

    public async Task<IEnumerable<PlayOffer>> Handle(GetPlayOffersByIdQuery request, CancellationToken cancellationToken) {

        var playOfferId = request.playOfferId;
        var creatorId = request.creatorId;
        var clubId = request.clubId;

        var result = await _context.PlayOffers.Where(po => po != null
                                                     && (!playOfferId.HasValue || po.Id == playOfferId)
                                                     && (!creatorId.HasValue || po.Creator.Id == creatorId)
                                                     && (!clubId.HasValue || po.Club.Id == clubId)
        ).ToListAsync();

        return result;
    }
}
