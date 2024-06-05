using PlayOfferService.Domain.Models;

namespace PlayOfferService.Tests.IntegrationTests;

[TestFixture]
public class MemberRepositoryTest : TestSetup
{
    [SetUp]
    public async Task MemberSetup()
    {
        var existingMember = new Member
        {
            Id = Guid.Parse("8bbb752a-784c-4fb1-9484-0522b4fb78d9"),
            ClubId = Guid.Parse("b0dd93aa-4b7d-4d36-89ed-be056976ca84"),
            Status = Status.ACTIVE
        };
        TestMemberRepository.CreateMember(existingMember);
        await TestMemberRepository.Update();
    }
    
    [Test]
    public async Task GetExistingMemberByIdTest()
    {
        //Given
        var memberId = Guid.Parse("8bbb752a-784c-4fb1-9484-0522b4fb78d9");
        
        //When
        var member = await TestMemberRepository.GetMemberById(memberId);
        
        //Then
        Assert.That(member, Is.Not.Null);
        Assert.That(member!.Id, Is.EqualTo(memberId));
    }
    
    [Test]
    public async Task GetNonExistingMemberByIdTest()
    {
        //Given
        var memberId = Guid.NewGuid();
        
        //When
        var member = await TestMemberRepository.GetMemberById(memberId);
        
        //Then
        Assert.That(member, Is.Null);
    }
    
    [Test]
    public async Task CreateMemberTest()
    {
        //Given
        var memberId = Guid.NewGuid();
        var newMember = new Member
        {
            Id = memberId,
            ClubId = Guid.Parse("b0dd93aa-4b7d-4d36-89ed-be056976ca84"),
            Status = Status.ACTIVE
        };
        
        //When
        TestMemberRepository.CreateMember(newMember);
        await TestMemberRepository.Update();
        
        //Then
        var member = await TestMemberRepository.GetMemberById(memberId);
        
        Assert.That(member, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(member!.Id, Is.EqualTo(memberId));
            Assert.That(member.ClubId, Is.EqualTo(Guid.Parse("b0dd93aa-4b7d-4d36-89ed-be056976ca84")));
            Assert.That(member.Status, Is.EqualTo(Status.ACTIVE));
        });
    }
}