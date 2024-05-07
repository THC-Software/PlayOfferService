﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using PlayOfferService.Models;
using PlayOfferService.Queries;
using PlayOfferService.Repositories;

namespace PlayOfferService.Handlers;
public class GetPlayOffersByIdHandler : IRequestHandler<GetPlayOffersByIdQuery, IEnumerable<PlayOffer>>
{
    private readonly PlayOfferRepository _playOfferRepository;

    public GetPlayOffersByIdHandler(PlayOfferRepository playOfferRepository)
    {
        _playOfferRepository = playOfferRepository;
    }

    public async Task<IEnumerable<PlayOffer>> Handle(GetPlayOffersByIdQuery request, CancellationToken cancellationToken)
    {
        return await _playOfferRepository.GetPlayOffersByIds(request.playOfferId, request.creatorId, request.clubId);
    }
}
