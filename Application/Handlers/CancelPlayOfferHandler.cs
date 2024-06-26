using MediatR;
using PlayOfferService.Application.Commands;
using PlayOfferService.Application.Exceptions;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.PlayOffer;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers;

public class CancelPlayOfferHandler : IRequestHandler<CancelPlayOfferCommand, Task>
{
    private readonly WriteEventRepository _writeEventRepository;
    private readonly PlayOfferRepository _playOfferRepository;
    private readonly ClubRepository _clubRepository;

    public CancelPlayOfferHandler(WriteEventRepository writeEventRepository, PlayOfferRepository playOfferRepository, ClubRepository clubRepository)
    {
        _writeEventRepository = writeEventRepository;
        _playOfferRepository = playOfferRepository;
        _clubRepository = clubRepository;
    }

    public async Task<Task> Handle(CancelPlayOfferCommand request, CancellationToken cancellationToken)
    {
        var transaction = _writeEventRepository.StartTransaction();
        var excpectedEventCount = _writeEventRepository.GetEventCount(request.PlayOfferId) + 1;

        var existingPlayOffer = (await _playOfferRepository.GetPlayOffersByIds(request.PlayOfferId)).FirstOrDefault();
        if (existingPlayOffer == null)
            throw new NotFoundException($"PlayOffer {request.PlayOfferId} not found!");
        if (existingPlayOffer.CreatorId != request.MemberId)
            throw new AuthorizationException($"PlayOffer {request.PlayOfferId} can only be cancelled by creator!");

        if (existingPlayOffer.OpponentId != null)
            throw new InvalidOperationException($"PlayOffer {request.PlayOfferId} is already accepted and cannot be cancelled!");
        if (existingPlayOffer.IsCancelled)
            throw new InvalidOperationException($"PlayOffer {request.PlayOfferId} is already cancelled!");

        var existingClub = await _clubRepository.GetClubById(existingPlayOffer.ClubId);
        if (existingClub == null)
            throw new NotFoundException($"Club {existingPlayOffer.ClubId} not found!");


        switch (existingClub.Status)
        {
            case Status.LOCKED:
                throw new InvalidOperationException("Can't cancel PlayOffer while club is locked!");
            case Status.DELETED:
                throw new InvalidOperationException("Can't cancel PlayOffer in deleted club!");
        }

        var domainEvent = new BaseEvent
        {
            EntityId = request.PlayOfferId,
            EntityType = EntityType.PLAYOFFER,
            EventId = Guid.NewGuid(),
            EventType = EventType.PLAYOFFER_CANCELLED,
            EventData = new PlayOfferCancelledEvent(),
            Timestamp = DateTime.UtcNow
        };

        await _writeEventRepository.AppendEvent(domainEvent);
        await _writeEventRepository.Update();
    
        
        var eventCount = _writeEventRepository.GetEventCount(request.PlayOfferId);
        
        if (eventCount != excpectedEventCount)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Concurrent modification detected!");
        }
        
        transaction.Commit();
        
        return Task.CompletedTask;
    }
}