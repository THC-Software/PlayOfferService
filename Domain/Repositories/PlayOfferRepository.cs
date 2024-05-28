using Microsoft.EntityFrameworkCore;
using PlayOfferService.Domain.Events;
using PlayOfferService.Models;

namespace PlayOfferService.Domain.Repositories;

public class PlayOfferRepository
{
    private readonly DbReadContext _context;
    private readonly ClubRepository _clubRepository;
    private readonly MemberRepository _memberRepository;
    
    public PlayOfferRepository(DbReadContext context, ClubRepository clubRepository, MemberRepository memberRepository)
    {
        _context = context;
        _clubRepository = clubRepository;
        _memberRepository = memberRepository;
    }
    
    public async Task<IEnumerable<PlayOffer>> GetPlayOffersByIds(
        Guid? playOfferId,
        Guid? creatorId = null,
        Guid? clubId = null)
    {
        var playOffers = await _context.PlayOffers
            .Include(playOffer => playOffer.Creator)
            .Include(playOffer => playOffer.Club)
            .ToListAsync();

        playOffers = playOffers.Where(e =>
            e != null
            && (!playOfferId.HasValue || e.Id == playOfferId)
            && (!creatorId.HasValue || e.Creator.Id == creatorId)
            && (!clubId.HasValue || e.Club.Id == clubId)).ToList();

        return playOffers;
    }

    public async Task UpdateEntityAsync(BaseEvent baseEvent)
    {
        Console.WriteLine("PlayOfferRepository received event: " + baseEvent.EventType);
        var appliedEvents = await _context.AppliedEvents
            .Where(e => e.EntityId == baseEvent.EntityId)
            .ToListAsync();
        
        if (appliedEvents.Any(e => e.EventId == baseEvent.EventId))
        {
            Console.WriteLine("Event already applied, skipping");
            return;
        }

        switch (baseEvent.EventType)
        {
            case EventType.PLAYOFFER_CANCELLED:
                await CancelPlayOffer(baseEvent);
                break;
            case EventType.PLAYOFFER_CREATED:
                await CreatePlayOffer(baseEvent);
                break;
            case EventType.PLAYOFFER_JOINED:
                await JoinPlayOffer(baseEvent);
                break;
        }

        _context.AppliedEvents.Add(baseEvent);
        await _context.SaveChangesAsync();
    }

    private async Task CancelPlayOffer(BaseEvent baseEvent)
    {
        var existingPlayOffer = (await GetPlayOffersByIds(baseEvent.EntityId)).First();
        existingPlayOffer.Apply([baseEvent]);
    }

    private async Task CreatePlayOffer(BaseEvent baseEvent)
    {
        var existingMember = await _memberRepository.GetMemberById(((PlayOfferCreatedEvent)baseEvent.EventData).Creator.Id);
        ((PlayOfferCreatedEvent)baseEvent.EventData).Creator = existingMember;
        
        var existingClub = await _clubRepository.GetClubById(((PlayOfferCreatedEvent)baseEvent.EventData).Club.Id);
        ((PlayOfferCreatedEvent)baseEvent.EventData).Club = existingClub;
        
        var newPlayOffer = new PlayOffer();
        newPlayOffer.Apply([baseEvent]);
        _context.PlayOffers.Add(newPlayOffer);
    }

    private async Task JoinPlayOffer(BaseEvent baseEvent)
    {
        var existingMember = await _memberRepository.GetMemberById(((PlayOfferJoinedEvent)baseEvent.EventData).Opponent.Id);
        ((PlayOfferJoinedEvent)baseEvent.EventData).Opponent = existingMember;
        
        var existingPlayOffer = (await GetPlayOffersByIds(baseEvent.EntityId)).First();
        existingPlayOffer.Apply([baseEvent]);
    }
}