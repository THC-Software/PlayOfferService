using System.ComponentModel.DataAnnotations.Schema;

namespace PlayOfferService.Models;

public class Club
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
}