using MediatR;
using PlayOfferService.Application.Queries;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers.Events;

public class GetPlayOffersByCreatorNameHandler : IRequestHandler<GetPlayOffersByCreatorNameQuery, IEnumerable<PlayOfferDto>>
{
    private readonly PlayOfferRepository _playOfferRepository;
    private readonly MemberRepository _memberRepository;
    private readonly ClubRepository _clubRepository;
    private readonly ReservationRepository _reservationRepository;
    private readonly CourtRepository _courtRepository;
    
    public GetPlayOffersByCreatorNameHandler(PlayOfferRepository playOfferRepository, MemberRepository memberRepository, ClubRepository clubRepository, ReservationRepository reservationRepository, CourtRepository courtRepository)
    {
        _playOfferRepository = playOfferRepository;
        _memberRepository = memberRepository;
        _clubRepository = clubRepository;
        _reservationRepository = reservationRepository;
        _courtRepository = courtRepository;
    }
    
    public async Task<IEnumerable<PlayOfferDto>> Handle(GetPlayOffersByCreatorNameQuery request, CancellationToken cancellationToken)
    {
        if (request.CreatorName.Split(" ").Length > 2)
            throw new ArgumentException("Creator name must be in the format '<FirstName> <LastName>', '<FirstName>' or '<LastName>'");
        
        var creators = await _memberRepository.GetMemberByName(request.CreatorName);
        var playOffers = new List<PlayOffer>();
        foreach (var creator in creators)
        {
            var playOffersByCreator = await _playOfferRepository.GetPlayOffersByIds(null, creator.Id);
            playOffers.AddRange(playOffersByCreator);
        }

        var clubDto = (await _clubRepository.GetAllClubs()).Select(club => new ClubDto(club)).ToList();
        var memberDtos = (await _memberRepository.GetAllMembers()).Select(member => new MemberDto(member)).ToList();
        var courtDtos = (await _courtRepository.GetAllCourts()).Select(court => new CourtDto(court)).ToList();
        var reservationDtos = (await _reservationRepository.GetAllReservations())
            .Select(reservation => new ReservationDto(
                reservation,
                courtDtos.First(courtDto => courtDto.Id == reservation.CourtId)))
            .ToList();
        
        var playOfferDtos = new List<PlayOfferDto>();
        foreach (var playOffer in playOffers)
        {
            var club = clubDto.First(club => club.Id == playOffer.ClubId);
            var creator = memberDtos.First(member => member.Id == playOffer.CreatorId);
            var opponent = memberDtos.FirstOrDefault(member => member.Id == playOffer.OpponentId);
            var reservation = reservationDtos.FirstOrDefault(reservation => reservation.Id == playOffer.ReservationId);
            playOfferDtos.Add(new PlayOfferDto(playOffer, club, creator, opponent, reservation));
        }

        return playOfferDtos;
    }
}