using MediatR;
using PlayOfferService.Domain.Models;

namespace PlayOfferService.Application.Commands;
public record JoinPlayOfferCommand(JoinPlayOfferDto JoinPlayOfferDto, Guid MemberId) : IRequest<Task>
{
}