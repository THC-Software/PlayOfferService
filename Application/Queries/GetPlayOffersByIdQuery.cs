using MediatR;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Application.Queries;
public record GetPlayOffersByIdQuery(Guid? PlayOfferId, Guid? CreatorId, Guid? ClubId) : IRequest<IEnumerable<PlayOffer>>
{
}
