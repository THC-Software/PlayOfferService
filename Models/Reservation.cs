using System.ComponentModel.DataAnnotations.Schema;

namespace PlayOfferService.Models;

public class Reservation
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
}