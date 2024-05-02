using MediatR;
using PlayOfferService.Models;

namespace PlayOfferService.Commands;
public record JoinPlayOfferCommand(JoinPlayOfferDto joinPlayOfferDto) : IRequest<Task>
{
}