namespace PlayOfferService.Domain.Models;

public class CourtDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public CourtDto(Court court)
    {
        Id = court.Id;
        Name = court.Name;
    }
}