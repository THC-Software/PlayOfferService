using System.ComponentModel.DataAnnotations.Schema;

namespace PlayOfferService.Models;

public class Reservation
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
}