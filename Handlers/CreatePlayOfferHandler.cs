using MediatR;
using PlayOfferService.Commands;
using PlayOfferService.Models;
using PlayOfferService.Repositories;

namespace PlayOfferService.Handlers;
public class CreatePlayOfferHandler : IRequestHandler<CreatePlayOfferCommand, PlayOffer> {

    private readonly DatabaseContext _context;

    public CreatePlayOfferHandler(DatabaseContext context) {
        _context = context;
    }

    public async Task<PlayOffer> Handle(CreatePlayOfferCommand request, CancellationToken cancellationToken) {
        var playOfferDto = request.playOfferDto;

        var playOffer = new PlayOffer(playOfferDto);

        _context.PlayOffers.Add(playOffer);
        await _context.SaveChangesAsync();

        return playOffer;
    }

}
