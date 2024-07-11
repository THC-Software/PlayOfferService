using MediatR;
using PlayOfferService.Application.Queries;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.Repositories;

namespace PlayOfferService.Application.Handlers;
public class GetPlayOffersByClubIdHandler : IRequestHandler<GetPlayOffersByClubIdQuery, IEnumerable<PlayOfferDto>>
{
    private readonly PlayOfferRepository _playOfferRepository;
    private readonly MemberRepository _memberRepository;
    private readonly ClubRepository _clubRepository;
    private readonly ReservationRepository _reservationRepository;
    private readonly CourtRepository _courtRepository;

    public GetPlayOffersByClubIdHandler(PlayOfferRepository playOfferRepository, MemberRepository memberRepository, ClubRepository clubRepository, ReservationRepository reservationRepository, CourtRepository courtRepository)
    {
        _playOfferRepository = playOfferRepository;
        _memberRepository = memberRepository;
        _clubRepository = clubRepository;
        _reservationRepository = reservationRepository;
        _courtRepository = courtRepository;
    }

    public async Task<IEnumerable<PlayOfferDto>> Handle(GetPlayOffersByClubIdQuery request, CancellationToken cancellationToken)
    {
        var playOffers = await _playOfferRepository.GetPlayOffersByIds(null, null, request.ClubId);
        var club = await _clubRepository.GetClubById(request.ClubId);
        
        var playOfferDtos = new List<PlayOfferDto>();
        if (club == null)
            return playOfferDtos;
        
        var clubDto = new ClubDto(club);
        var memberDtos = (await _memberRepository.GetAllMembers()).Select(member => new MemberDto(member)).ToList();
        var courtDtos = (await _courtRepository.GetAllCourts()).Select(court => new CourtDto(court)).ToList();
        var reservationDtos = (await _reservationRepository.GetAllReservations())
            .Select(reservation => new ReservationDto(
                reservation,
                courtDtos.First(courtDto => courtDto.Id == reservation.CourtId)))
            .ToList();
        
        
        foreach (var playOffer in playOffers)
        {
            var creator = memberDtos.First(member => member.Id == playOffer.CreatorId);
            var opponent = memberDtos.FirstOrDefault(member => member.Id == playOffer.OpponentId);
            var reservation = reservationDtos.FirstOrDefault(reservation => reservation.Id == playOffer.ReservationId);
            playOfferDtos.Add(new PlayOfferDto(playOffer, clubDto, creator, opponent, reservation));
        }

        return playOfferDtos;
    }
}
