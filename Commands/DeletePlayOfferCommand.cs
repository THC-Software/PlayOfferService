using MediatR;

namespace PlayOfferService.Commands;

public record DeletePlayOfferCommand(Guid playOfferId) : IRequest
{

}
