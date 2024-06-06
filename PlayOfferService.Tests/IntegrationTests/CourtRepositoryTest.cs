using PlayOfferService.Domain.Models;

namespace PlayOfferService.Tests.IntegrationTests;

public class CourtRepositoryTest : TestSetup
{
        [SetUp]
    public async Task ReservationSetup()
    {
        var existingCourt = new Court
        {
            Id = Guid.Parse("156a5115-98ef-4e4e-a9c2-b1c4beac4e92"),
            ClubId = Guid.Parse("8b5a51c8-5945-446f-8be6-6372eaacc542"),
            Name = "Court 42"
        };
        TestCourtRepository.CreateCourt(existingCourt);
        await TestReservationRepository.Update();
    }
    
    [Test]
    public async Task GetExistingReservationByIdTest()
    {
        //Given
        var courtId = Guid.Parse("156a5115-98ef-4e4e-a9c2-b1c4beac4e92");
        
        //When
        var court = await TestCourtRepository.GetCourtById(courtId);
        
        //Then
        Assert.That(court, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(court!.Id, Is.EqualTo(courtId));
            Assert.That(court.ClubId, Is.EqualTo(Guid.Parse("8b5a51c8-5945-446f-8be6-6372eaacc542")));
            Assert.That(court.Name, Is.EqualTo("Court 42"));
        });
    }
    
    [Test]
    public async Task GetNonExistingReservationByIdTest()
    {
        //Given
        var courtId = Guid.NewGuid();
        
        //When
        var court = await TestCourtRepository.GetCourtById(courtId);
        
        //Then
        Assert.That(court, Is.Null);
    }
    
    [Test]
    public async Task CreateReservationTest()
    {
        //Given
        var givenCourt = new Court
        {
            Id = Guid.Parse("4a857292-7957-4731-8b89-9d8899613139"),
            ClubId = Guid.Parse("a534cb67-3254-40f6-a661-316788e7a5f1"),
            Name = "Court 1"
        };
        
        //When
        TestCourtRepository.CreateCourt(givenCourt);
        await TestCourtRepository.Update();
        
        //Then
        var court = await TestCourtRepository.GetCourtById(givenCourt.Id);
        
        Assert.That(court, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(court!.Id, Is.EqualTo(Guid.Parse("4a857292-7957-4731-8b89-9d8899613139")));
            Assert.That(court.ClubId, Is.EqualTo(Guid.Parse("a534cb67-3254-40f6-a661-316788e7a5f1")));
            Assert.That(court.Name, Is.EqualTo("Court 1"));
        });
    }
}