using System.ComponentModel.DataAnnotations.Schema;

namespace PlayOfferService.Domain.Models;

public class Reservation
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
}