using MediatR;

namespace PlayOfferService.Commands;

public record CancelPlayOfferCommand(Guid playOfferId) : IRequest<Task>
{
}
