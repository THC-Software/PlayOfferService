using MediatR;
using PlayOfferService.Commands;
using PlayOfferService.Repositories;

namespace PlayOfferService.Handlers;

public class DeletePlayOfferHandler : IRequestHandler<DeletePlayOfferCommand>
{
    private readonly DatabaseContext _context;

    public DeletePlayOfferHandler(DatabaseContext context)
    {
        _context = context;
    }


    public async Task Handle(DeletePlayOfferCommand request, CancellationToken cancellationToken)
    {
        var playOffer = _context.PlayOffers.FirstOrDefault(po => po.Id == request.playOfferId);

        if (playOffer == null)
        {
            await Task.FromException(new NullReferenceException());
        }

        _context.PlayOffers.Remove(playOffer);

        await _context.SaveChangesAsync();

        await Task.CompletedTask;
    }
}