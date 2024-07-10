using System.ComponentModel.DataAnnotations;

namespace PlayOfferService.Domain.Models;

public class CreatePlayOfferDto
{
    [Required]
    public DateTime ProposedStartTime { get; set; }
    [Required]
    public DateTime ProposedEndTime { get; set; }
}