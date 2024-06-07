using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Domain.Models;
using PlayOfferService.Domain.ValueObjects;

namespace PlayOfferService.Tests.IntegrationTests;

public class MemberEventHandlerTest : TestSetup
{
    [SetUp]
    public async Task MemberSetup()
    {
        var existingMembers = new List<Member>
        {
            new()
            {
                Id = Guid.Parse("971c48ff-42f4-4dc5-94ad-42e3c155b07b"),
                ClubId = Guid.Parse("bf7f59db-e2bf-4a4f-95fe-baeabe948b81"),
                Status = Status.ACTIVE
            },
            new()
            {
                Id = Guid.Parse("c559d8ad-67be-4739-afb0-94460d7bb100"),
                ClubId = Guid.Parse("8ad9ddaa-8ce6-4655-b2da-fb3419f93a67"),
                Status = Status.ACTIVE
            }
        };
        
        foreach (var member in existingMembers)
        {
            TestMemberRepository.CreateMember(member);
        }
        await TestMemberRepository.Update();

        var existingPlayOffers = new List<PlayOffer>
        {
            new()
            {
                Id = Guid.Parse("033cad0e-6270-487c-85e2-02a7b531b1cd"),
                ClubId = Guid.Parse("c7545dea-2b37-4ee9-b99b-7ee422565870"),
                CreatorId = Guid.Parse("971c48ff-42f4-4dc5-94ad-42e3c155b07b"),
                ProposedStartTime = DateTime.UtcNow,
                ProposedEndTime = DateTime.UtcNow.AddHours(3),
                IsCancelled = false
            },
            new()
            {
                Id = Guid.Parse("301289c5-6789-4a64-b9f6-37a141191123"),
                ClubId = Guid.Parse("7b219549-58fb-4018-8958-151eacab2d1c"),
                CreatorId = Guid.Parse("c559d8ad-67be-4739-afb0-94460d7bb100"),
                ProposedStartTime = DateTime.UtcNow,
                ProposedEndTime = DateTime.UtcNow.AddHours(3),
                IsCancelled = false
            },
        };
        
        foreach (var playOffer in existingPlayOffers)
        {
            TestPlayOfferRepository.CreatePlayOffer(playOffer);
        }
        
        await TestPlayOfferRepository.Update();
    }

    [Test]
    public async Task MemberCreatedEvent_ProjectionTest()
    {
        //Given
        var memberId = Guid.NewGuid();
        var memberCreatedEvent = new TechnicalMemberEvent
        {
            EntityId = memberId,
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_REGISTERED,
            EventData = new MemberCreatedEvent
            {
                MemberId = new MemberId { Id = memberId },
                Name = new FullName { FirstName = "Test", LastName = "Member" },
                TennisClubId = new TennisClubId { Id = Guid.Parse("bf7f59db-e2bf-4a4f-95fe-baeabe948b81") }
            }
        };

        //When
        await Mediator.Send(memberCreatedEvent);

        //Then
        var projectedMember = await TestMemberRepository.GetMemberById(memberId);

        Assert.That(projectedMember, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedMember!.Id, Is.EqualTo(memberId));
            Assert.That(projectedMember.ClubId, Is.EqualTo(Guid.Parse("bf7f59db-e2bf-4a4f-95fe-baeabe948b81")));
            Assert.That(projectedMember.Status, Is.EqualTo(Status.ACTIVE));
        });
    }

    [Test]
    public async Task MemberLockEvent_ProjectionTest()
    {
        //Given
        var memberLockEvent = new TechnicalMemberEvent
        {
            EntityId = Guid.Parse("971c48ff-42f4-4dc5-94ad-42e3c155b07b"),
            EntityType = EntityType.MEMBER,
            EventId = Guid.Parse("d1f2c32e-77b3-4e3b-a632-7ea710e93b44"),
            EventType = EventType.MEMBER_LOCKED,
            EventData = new MemberLockedEvent()
        };

        //When
        await Mediator.Send(memberLockEvent);

        //Then
        var projectedMember = await TestMemberRepository.GetMemberById(Guid.Parse("971c48ff-42f4-4dc5-94ad-42e3c155b07b"));
        
        Assert.That(projectedMember, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedMember!.Id, Is.EqualTo(Guid.Parse("971c48ff-42f4-4dc5-94ad-42e3c155b07b")));
            Assert.That(projectedMember.Status, Is.EqualTo(Status.LOCKED));
        });
        
        var playOfferEvents = await TestWriteEventRepository.GetEventByEntityId(Guid.Parse("033cad0e-6270-487c-85e2-02a7b531b1cd"));
        Assert.That(playOfferEvents, Has.Count.EqualTo(1));
        
        Assert.Multiple(() =>
        {
            Assert.That(playOfferEvents.First().EntityId, Is.EqualTo(Guid.Parse("033cad0e-6270-487c-85e2-02a7b531b1cd")));
            Assert.That(playOfferEvents.First().EventType, Is.EqualTo(EventType.PLAYOFFER_CANCELLED));
            Assert.That(playOfferEvents.First().CorrelationId, Is.EqualTo(Guid.Parse("d1f2c32e-77b3-4e3b-a632-7ea710e93b44")));
        });
    }

    [Test]
    public async Task MemberUnlockEvent_ProjectionTest()
    {
        //Given
        var existingMember = await TestMemberRepository.GetMemberById(Guid.Parse("971c48ff-42f4-4dc5-94ad-42e3c155b07b"));
        var memberUnlockEvent = new TechnicalMemberEvent
        {
            EntityId = existingMember!.Id,
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_UNLOCKED,
            EventData = new MemberUnlockedEvent()
        };

        //When
        await Mediator.Send(memberUnlockEvent);

        //Then
        var projectedMember = await TestMemberRepository.GetMemberById(existingMember.Id);

        Assert.That(projectedMember, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedMember!.Id, Is.EqualTo(existingMember.Id));
            Assert.That(projectedMember.Status, Is.EqualTo(Status.ACTIVE));
        });
    }

    [Test]
    public async Task MemberDeletedEvent_ProjectionTest()
    {
        //Given
        var memberDeleteEvent = new TechnicalMemberEvent
        {
            EntityId = Guid.Parse("c559d8ad-67be-4739-afb0-94460d7bb100"),
            EntityType = EntityType.MEMBER,
            EventId = Guid.Parse("367d90df-ef2e-44cf-a6e1-704fbafbb561"),
            EventType = EventType.MEMBER_DELETED,
            EventData = new MemberDeletedEvent()
        };

        //When
        await Mediator.Send(memberDeleteEvent);

        //Then
        var projectedMember = await TestMemberRepository.GetMemberById(Guid.Parse("c559d8ad-67be-4739-afb0-94460d7bb100"));
        
        Assert.That(projectedMember, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedMember!.Id, Is.EqualTo(Guid.Parse("c559d8ad-67be-4739-afb0-94460d7bb100")));
            Assert.That(projectedMember.Status, Is.EqualTo(Status.DELETED));
        });
        
        var playOfferEvents = await TestWriteEventRepository.GetEventByEntityId(Guid.Parse("301289c5-6789-4a64-b9f6-37a141191123"));
        Assert.That(playOfferEvents, Has.Count.EqualTo(1));
        
        Assert.Multiple(() =>
        {
            Assert.That(playOfferEvents.First().EntityId, Is.EqualTo(Guid.Parse("301289c5-6789-4a64-b9f6-37a141191123")));
            Assert.That(playOfferEvents.First().EventType, Is.EqualTo(EventType.PLAYOFFER_CANCELLED));
            Assert.That(playOfferEvents.First().CorrelationId, Is.EqualTo(Guid.Parse("367d90df-ef2e-44cf-a6e1-704fbafbb561")));
        });
    }
}