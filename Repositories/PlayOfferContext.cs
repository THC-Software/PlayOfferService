using Microsoft.EntityFrameworkCore;
using PlayOfferService.Models;

namespace PlayOfferService.Repositories;

public class PlayOfferContext :DbContext
{
    public DbSet<PlayOffer> PlayOffers { get; set; }
}