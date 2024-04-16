using System.ComponentModel.DataAnnotations.Schema;

namespace PlayOfferService.Models;

public class Member
{
    public Guid Id { get; set; }
    public Club Club { get; set; }
}