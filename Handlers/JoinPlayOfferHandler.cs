using MediatR;
using Microsoft.EntityFrameworkCore;
using PlayOfferService.Commands;
using PlayOfferService.Models;
using PlayOfferService.Repositories;

namespace PlayOfferService.Handlers;

public class JoinPlayOfferHandler : IRequestHandler<JoinPlayOfferCommand, Task>
{
    public readonly DatabaseContext _context;

    public JoinPlayOfferHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Task> Handle(JoinPlayOfferCommand request, CancellationToken cancellationToken)
    {
        var playOffer = await _context.PlayOffers.FirstOrDefaultAsync(po => po.Id == request.joinPlayOfferDto.PlayOfferId);

        if (playOffer == null)
        {
            return Task.FromException(new NullReferenceException());
        }

        playOffer.Opponent = new Member { Id = request.joinPlayOfferDto.OpponentId };

        await _context.SaveChangesAsync();

        return Task.CompletedTask;
    }
}
