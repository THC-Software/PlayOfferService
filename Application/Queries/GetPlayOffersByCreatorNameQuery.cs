using MediatR;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Application.Queries;

public record GetPlayOffersByCreatorNameQuery(string CreatorName) : IRequest<IEnumerable<PlayOfferDto>>
{
}