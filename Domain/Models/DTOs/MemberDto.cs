namespace PlayOfferService.Domain.Models;

public class MemberDto
{
    public Guid Id { get; set; }

    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public Status Status { get; set; }
    
    public MemberDto(Member member)
    {
        Id = member.Id;
        Email = member.Email;
        FirstName = member.FirstName;
        LastName = member.LastName;
        Status = member.Status;
    }
}