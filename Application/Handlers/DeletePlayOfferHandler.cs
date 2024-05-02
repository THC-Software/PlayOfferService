using MediatR;
using PlayOfferService.Commands;
using PlayOfferService.Repositories;

namespace PlayOfferService.Handlers;

public class DeletePlayOfferHandler : IRequestHandler<DeletePlayOfferCommand, Task>
{
    private readonly DatabaseContext _context;

    public DeletePlayOfferHandler(DatabaseContext context)
    {
        _context = context;
    }


    public async Task<Task> Handle(DeletePlayOfferCommand request, CancellationToken cancellationToken)
    {
        var playOffer = _context.PlayOffers.FirstOrDefault(po => po.Id == request.playOfferId);

        if (playOffer == null)
        {
            return Task.FromException(new NullReferenceException());
        }

        _context.PlayOffers.Remove(playOffer);

        await _context.SaveChangesAsync();

        return Task.CompletedTask;
    }
}