using System.ComponentModel.DataAnnotations;

namespace PlayOfferService.Domain.Models;

public class JoinPlayOfferDto
{
    [Required]
    public Guid PlayOfferId { get; set; }
    [Required]
    public DateTime AcceptedStartTime { get; set; }
}