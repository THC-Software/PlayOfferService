using System.ComponentModel.DataAnnotations.Schema;

namespace PlayOfferService.Models;

public class Member
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public Club Club { get; set; }
}