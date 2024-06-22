using MediatR;

namespace PlayOfferService.Application.Commands;

public record CancelPlayOfferCommand(Guid PlayOfferId, Guid MemberId) : IRequest<Task>
{
}
