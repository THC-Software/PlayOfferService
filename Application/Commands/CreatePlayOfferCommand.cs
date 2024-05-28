using MediatR;
using PlayOfferService.Models;

namespace PlayOfferService.Commands;
public record CreatePlayOfferCommand(PlayOfferDto playOfferDto) : IRequest<Guid>
{
}