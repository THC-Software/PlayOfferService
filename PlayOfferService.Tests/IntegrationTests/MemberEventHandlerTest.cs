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
        var existingMember = new Member
        {
            Id = Guid.Parse("971c48ff-42f4-4dc5-94ad-42e3c155b07b"),
            ClubId = Guid.Parse("bf7f59db-e2bf-4a4f-95fe-baeabe948b81"),
            Status = Status.ACTIVE
        };
        TestMemberRepository.CreateMember(existingMember);
        await TestMemberRepository.Update();
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
                MemberId = new MemberId{Id=memberId},
                TennisClubId = new TennisClubId{Id=Guid.Parse("bf7f59db-e2bf-4a4f-95fe-baeabe948b81")}
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
        var existingMember = await TestMemberRepository.GetMemberById(Guid.Parse("971c48ff-42f4-4dc5-94ad-42e3c155b07b"));
        var memberLockEvent = new TechnicalMemberEvent
        {
            EntityId = existingMember!.Id,
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_LOCKED,
            EventData = new MemberLockedEvent()
        };
        
        //When
        await Mediator.Send(memberLockEvent);
        
        //Then
        var projectedMember = await TestMemberRepository.GetMemberById(existingMember.Id);
        
        Assert.That(projectedMember, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedMember!.Id, Is.EqualTo(existingMember.Id));
            Assert.That(projectedMember.Status, Is.EqualTo(Status.LOCKED));
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
        var existingMember = await TestMemberRepository.GetMemberById(Guid.Parse("971c48ff-42f4-4dc5-94ad-42e3c155b07b"));
        var memberDeleteEvent = new TechnicalMemberEvent
        {
            EntityId = existingMember!.Id,
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_DELETED,
            EventData = new MemberDeletedEvent()
        };
        
        //When
        await Mediator.Send(memberDeleteEvent);
        
        //Then
        var projectedMember = await TestMemberRepository.GetMemberById(existingMember.Id);
        
        Assert.That(projectedMember, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedMember!.Id, Is.EqualTo(existingMember.Id));
            Assert.That(projectedMember.Status, Is.EqualTo(Status.DELETED));
        });
    }
}