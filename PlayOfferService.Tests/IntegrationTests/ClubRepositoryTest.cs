using PlayOfferService.Domain.Models;

namespace PlayOfferService.Tests.IntegrationTests;

[TestFixture]
public class ClubRepositoryTest : TestSetup
{
    [SetUp]
    public async Task ClubSetup()
    {
        var existingClub = new Club
        {
            Id = Guid.Parse("67bc285f-1b28-40f9-8b9e-564b3d9c1297"),
            Status = Status.ACTIVE
        };
        TestClubRepository.CreateClub(existingClub);
        await TestClubRepository.Update();
    }

    [Test]
    public async Task GetExistingClubByIdTest()
    {
        //Given
        var clubId = Guid.Parse("67bc285f-1b28-40f9-8b9e-564b3d9c1297");
        
        //When
        var club = await TestClubRepository.GetClubById(clubId);
        
        //Then
        Assert.That(club, Is.Not.Null);
        Assert.That(club!.Id, Is.EqualTo(clubId));
    }
    
    [Test]
    public async Task GetNonExistingClubByIdTest()
    {
        //Given
        var clubId = Guid.NewGuid();
        
        //When
        var club = await TestClubRepository.GetClubById(clubId);
        
        //Then
        Assert.That(club, Is.Null);
    }
    
    [Test]
    public async Task CreateClubTest()
    {
        //Given
        var clubId = Guid.NewGuid();
        var newClub = new Club
        {
            Id = clubId,
            Status = Status.ACTIVE
        };
        
        //When
        TestClubRepository.CreateClub(newClub);
        await TestClubRepository.Update();
        
        //Then
        var club = await TestClubRepository.GetClubById(clubId);
        Assert.That(club, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(club!.Id, Is.EqualTo(clubId));
            Assert.That(club.Status, Is.EqualTo(Status.ACTIVE));
        });
    }
}