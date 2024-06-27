using MediatR;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Application.Queries;
public record GetPlayOffersByClubIdQuery(Guid ClubId) : IRequest<IEnumerable<PlayOfferDto>>
{
}
