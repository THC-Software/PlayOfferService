using MediatR;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Application.Queries;

public record GetPlayOffersByParticipantIdQuery(Guid ParticipantId) : IRequest<IEnumerable<PlayOfferDto>>
{
}