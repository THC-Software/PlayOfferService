using MediatR;
using PlayOfferService.Models;

namespace PlayOfferService.Queries;
public record GetPlayOffersByIdQuery(Guid? playOfferId, Guid? creatorId, Guid? clubId) : IRequest<IEnumerable<PlayOffer>>
{
}
