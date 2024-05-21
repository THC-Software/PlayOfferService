using PlayOfferService.Domain.Events;
using PlayOfferService.Repositories;

namespace PlayOfferService.Tests.IntegrationTests;

[TestFixture]
public class ClubRepositoryTest : TestSetup
{
    [Test]
    public async Task ClubCreatedEvent_ProjectionTest()
    {
        //Given
        var clubId = Guid.NewGuid();
        var clubCreationEvent = new BaseEvent
        {
            EntityId = clubId,
            EntityType = EntityType.CLUB,
            EventId = Guid.NewGuid(),
            EventType = EventType.TENNIS_CLUB_REGISTERED,
            EventData = new ClubCreatedEvent
            {
                TennisClubId = clubId
            }
        };
        
        //When
        await TestClubRepository.UpdateEntityAsync(clubCreationEvent);
        
        //Then
        var projectedClub = await TestClubRepository.GetClubById(clubId);
        
        Assert.That(projectedClub, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedClub.Id, Is.EqualTo(clubId));
            Assert.That(projectedClub.IsLocked, Is.False);
        });

    }
}