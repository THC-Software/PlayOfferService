namespace PlayOfferService.Domain.Models;

public class ClubDto : Club
{
    public ClubDto(Club club)
    {
        Id = club.Id;
        Name = club.Name;
        Status = club.Status;
    }
}