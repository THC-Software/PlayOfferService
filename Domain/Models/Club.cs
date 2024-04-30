using System.ComponentModel.DataAnnotations.Schema;

namespace PlayOfferService.Models;

public class Club
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
}