using MediatR;
using PlayOfferService.Commands;
using PlayOfferService.Domain;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers;

public class JoinPlayOfferHandler : IRequestHandler<JoinPlayOfferCommand, Task>
{
    private readonly DbWriteContext _context;
    private readonly PlayOfferRepository _playOfferRepository;
    private readonly MemberRepository _memberRepository;

    public JoinPlayOfferHandler(DbWriteContext context, PlayOfferRepository playOfferRepository, MemberRepository memberRepository)
    {
        _context = context;
        _playOfferRepository = playOfferRepository;
        _memberRepository = memberRepository;
    }

    public async Task<Task> Handle(JoinPlayOfferCommand request, CancellationToken cancellationToken)
    {
        var existingPlayOffers = await _playOfferRepository.GetPlayOffersByIds(request.joinPlayOfferDto.PlayOfferId);
        if (existingPlayOffers.ToList().Count == 0)
            throw new ArgumentException("PlayOffer not found with id: " + request.joinPlayOfferDto.PlayOfferId);

        var existingOpponent = await _memberRepository.GetMemberById(request.joinPlayOfferDto.OpponentId);

        var domainEvent = new BaseEvent
        {
            EntityId = request.joinPlayOfferDto.PlayOfferId,
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.NewGuid(),
            EventType = EventType.PLAYOFFER_JOINED,
            EventData = new PlayOfferJoinedEvent
            {
                Opponent = existingOpponent,
                AcceptedStartTime = request.joinPlayOfferDto.AcceptedStartTime.ToUniversalTime(),
            },
            Timestamp = DateTime.UtcNow
        };

        existingPlayOffers.First().Apply([domainEvent]);

        _context.Events.Add(domainEvent);
        await _context.SaveChangesAsync();

        return Task.CompletedTask;
    }
}
