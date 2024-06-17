using PlayOfferService.Domain.Models;

namespace PlayOfferService.Tests.IntegrationTests;

public class PlayOfferRepositoryTest : TestSetup
{
    [SetUp]
    public async Task PlayOfferSetup()
    {
        var existingPlayOffers = new List<PlayOffer> {
            new()
            {
                Id = Guid.Parse("d79f0bd6-c7ec-44e5-a02f-26e1567b0992"),
                ClubId = Guid.Parse("34f13619-14b5-4244-a74b-6a8ba210a0b1"),
                CreatorId = Guid.Parse("9b30e631-3ad8-4437-a934-94252d6294c4"),
                ReservationId = Guid.Parse("b5fab7b7-63f1-4847-a71c-ae37dbfed029")
            },
            new()
            {
                Id = Guid.Parse("9b0e6c95-1a00-49a0-9414-f83ed0eb388f"),
                ClubId = Guid.Parse("34f13619-14b5-4244-a74b-6a8ba210a0b1"),
                CreatorId = Guid.Parse("4fe95e2d-a943-444e-99aa-396d141b03ec"),
            },
            new()
            {
                Id = Guid.Parse("d71ad67e-fa99-4ef2-b3ee-c6be640607a8"),
                ClubId = Guid.Parse("34f13619-14b5-4244-a74b-6a8ba210a0b1"),
                CreatorId = Guid.Parse("9b30e631-3ad8-4437-a934-94252d6294c4"),
                OpponentId = Guid.Parse("4fe95e2d-a943-444e-99aa-396d141b03ec"),
            }
        };
        foreach (var playOffer in existingPlayOffers)
        {
            TestPlayOfferRepository.CreatePlayOffer(playOffer);
        }
        await TestPlayOfferRepository.Update();
    }
    
    [Test]
    public async Task GetExistingPlayOfferByIdTest()
    {
        //Given
        var playOfferId = Guid.Parse("d79f0bd6-c7ec-44e5-a02f-26e1567b0992");
        
        //When
        var playOffers = (await TestPlayOfferRepository.GetPlayOffersByIds(playOfferId)).ToList();
        
        //Then
        Assert.That(playOffers, Is.Not.Null);
        Assert.That(playOffers, Has.Count.EqualTo(1));
        Assert.That(playOffers.First().Id, Is.EqualTo(playOfferId));
    }
    
    [Test]
    public async Task GetNonExistingPlayOfferByIdTest()
    {
        //Given
        var playOfferId = Guid.NewGuid();
        
        //When
        var playOffers = (await TestPlayOfferRepository.GetPlayOffersByIds(playOfferId)).ToList();
        
        //Then
        Assert.That(playOffers, Is.Not.Null);
        Assert.That(playOffers, Is.Empty);
    }

    [Test]
    public async Task GetPlayOffersByClubIdTest()
    {
        //Given
        var clubId = Guid.Parse("34f13619-14b5-4244-a74b-6a8ba210a0b1");
        
        //When
        var playOffers = (await TestPlayOfferRepository.GetPlayOffersByIds(null,null,clubId)).ToList();
        
        //Then
        Assert.That(playOffers, Is.Not.Null);
        Assert.That(playOffers, Has.Count.EqualTo(3));
    }
    
    [Test]
    public async Task GetPlayOffersByCreatorIdTest()
    {
        //Given
        var creatorId = Guid.Parse("9b30e631-3ad8-4437-a934-94252d6294c4");
        
        //When
        var playOffers = (await TestPlayOfferRepository.GetPlayOffersByIds(null,creatorId)).ToList();
        
        //Then
        Assert.That(playOffers, Is.Not.Null);
        Assert.That(playOffers, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(playOffers[1].Id, Is.EqualTo(Guid.Parse("d79f0bd6-c7ec-44e5-a02f-26e1567b0992")));
            Assert.That(playOffers[0].Id, Is.EqualTo(Guid.Parse("d71ad67e-fa99-4ef2-b3ee-c6be640607a8")));
        });
    }
    
    [Test]

    public async Task GetPlayOffersByOpponentIdTest()
    {
        //Given
        var opponentId = Guid.Parse("4fe95e2d-a943-444e-99aa-396d141b03ec");
        
        //When
        var playOffers = (await TestPlayOfferRepository.GetPlayOffersByIds(null,null,null,opponentId)).ToList();
        
        //Then
        Assert.That(playOffers, Is.Not.Null);
        Assert.That(playOffers, Has.Count.EqualTo(1));
        Assert.That(playOffers.First().Id, Is.EqualTo(Guid.Parse("d71ad67e-fa99-4ef2-b3ee-c6be640607a8")));
    }

    [Test]
    public async Task GetPlayOffersByParticipantId()
    {
        //Given
        var participantId = Guid.Parse("4fe95e2d-a943-444e-99aa-396d141b03ec");
        
        //When
        var playOffers = (await TestPlayOfferRepository.GetPlayOffersByParticipantId(participantId)).ToList();
        
        //Then
        Assert.That(playOffers, Is.Not.Null);
        Assert.That(playOffers, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(playOffers[0].Id, Is.EqualTo(Guid.Parse("9b0e6c95-1a00-49a0-9414-f83ed0eb388f")));
            Assert.That(playOffers[1].Id, Is.EqualTo(Guid.Parse("d71ad67e-fa99-4ef2-b3ee-c6be640607a8")));
        });
    }
    
    [Test]
    public async Task CreatePlayOfferTest()
    {
        //Given
        var playOfferId = Guid.NewGuid();
        var newPlayOffer = new PlayOffer
        {
            Id = playOfferId,
            ClubId = Guid.Parse("34f13619-14b5-4244-a74b-6a8ba210a0b1"),
            CreatorId = Guid.Parse("9b30e631-3ad8-4437-a934-94252d6294c4"),
        };
        
        //When
        TestPlayOfferRepository.CreatePlayOffer(newPlayOffer);
        await TestPlayOfferRepository.Update();
        
        //Then
        var playOffers = (await TestPlayOfferRepository.GetPlayOffersByIds(playOfferId)).ToList();
        
        Assert.That(playOffers, Is.Not.Null);
        Assert.That(playOffers, Has.Count.EqualTo(1));
        Assert.That(playOffers.First().Id, Is.EqualTo(playOfferId));
    }
}