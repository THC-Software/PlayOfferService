using MediatR;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Application.Commands;
public record CreatePlayOfferCommand(CreatePlayOfferDto CreatePlayOfferDto) : IRequest<Guid>
{
}