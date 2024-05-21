using NUnit.Framework.Internal;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Models;

namespace PlayOfferService.Tests.IntegrationTests;

[TestFixture]
public class MemberRepositoryTest : TestSetup
{
    [SetUp]
    public async Task MemberSetup()
    {
        var clubCreationEvent = new BaseEvent
        {
            EntityId = Guid.Parse("0411624f-ac2a-4ce8-a02f-b8978bb6d84d"),
            EntityType = EntityType.CLUB,
            EventId = Guid.NewGuid(),
            EventType = EventType.TENNIS_CLUB_REGISTERED,
            EventData = new ClubCreatedEvent
            {
                TennisClubId = Guid.Parse("0411624f-ac2a-4ce8-a02f-b8978bb6d84d")
            }
        };
        await TestClubRepository.UpdateEntityAsync(clubCreationEvent);
        
        var existingClub = await TestClubRepository.GetClubById(Guid.Parse("0411624f-ac2a-4ce8-a02f-b8978bb6d84d"));
        var memberCreationEvent = new BaseEvent
        {
            EntityId = Guid.Parse("16a1a213-f684-4e08-b9ef-61b372c59bf4"),
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_ACCOUNT_CREATED,
            EventData = new MemberCreatedEvent
            {
                MemberAccountId = Guid.Parse("16a1a213-f684-4e08-b9ef-61b372c59bf4"),
                Club = existingClub
            }
        };
        await TestMemberRepository.UpdateEntityAsync(memberCreationEvent);
        
        var lockedMemberCreationEvent = new BaseEvent
        {
            EntityId = Guid.Parse("d920f6c9-e328-4e84-be64-0a586269f89d"),
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_ACCOUNT_CREATED,
            EventData = new MemberCreatedEvent
            {
                MemberAccountId = Guid.Parse("d920f6c9-e328-4e84-be64-0a586269f89d"),
                Club = existingClub
            }
        };
        var memberLockEvent = new BaseEvent
        {
            EntityId = Guid.Parse("d920f6c9-e328-4e84-be64-0a586269f89d"),
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_ACCOUNT_LOCKED,
            EventData = new MemberLockedEvent()
        };
        await TestMemberRepository.UpdateEntityAsync(lockedMemberCreationEvent);
        await TestMemberRepository.UpdateEntityAsync(memberLockEvent);
        
    }
    
    [Test]
    public async Task MemberCreatedEvent_ProjectionTest()
    {
        //Given
        var existingClub = await TestClubRepository.GetClubById(Guid.Parse("0411624f-ac2a-4ce8-a02f-b8978bb6d84d"));
        
        var memberId = Guid.NewGuid();
        var memberCreatedEvent = new BaseEvent
        {
            EntityId = memberId,
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_ACCOUNT_CREATED,
            EventData = new MemberCreatedEvent
            {
                MemberAccountId = memberId,
                Club = existingClub
            }
        };
        
        //When
        await TestMemberRepository.UpdateEntityAsync(memberCreatedEvent);
        
        //Then
        var projectedMember = await TestMemberRepository.GetMemberById(memberId);
        
        Assert.That(projectedMember, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedMember.Id, Is.EqualTo(memberId));
            Assert.That(projectedMember.Club.Id, Is.EqualTo(Guid.Parse("0411624f-ac2a-4ce8-a02f-b8978bb6d84d")));
            Assert.That(projectedMember.IsLocked, Is.False);
        });
    }
    
    [Test]
    public async Task MemberLockEvent_ProjectionTest()
    {
        //Given
        var existingMember = await TestMemberRepository.GetMemberById(Guid.Parse("16a1a213-f684-4e08-b9ef-61b372c59bf4"));
        var memberLockEvent = new BaseEvent
        {
            EntityId = existingMember.Id,
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_ACCOUNT_LOCKED,
            EventData = new MemberLockedEvent()
        };
        
        //When
        await TestMemberRepository.UpdateEntityAsync(memberLockEvent);
        
        //Then
        var projectedMember = await TestMemberRepository.GetMemberById(existingMember.Id);
        
        Assert.That(projectedMember, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedMember.Id, Is.EqualTo(existingMember.Id));
            Assert.That(projectedMember.Club.Id, Is.EqualTo(Guid.Parse("0411624f-ac2a-4ce8-a02f-b8978bb6d84d")));
            Assert.That(projectedMember.IsLocked, Is.True);
        });
    }
    
    [Test]
    public async Task MemberUnlockEvent_ProjectionTest()
    {
        //Given
        var existingMember = await TestMemberRepository.GetMemberById(Guid.Parse("d920f6c9-e328-4e84-be64-0a586269f89d"));
        var memberUnlockEvent = new BaseEvent
        {
            EntityId = existingMember.Id,
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_ACCOUNT_UNLOCKED,
            EventData = new MemberUnlockedEvent()
        };
        
        //When
        await TestMemberRepository.UpdateEntityAsync(memberUnlockEvent);
        
        //Then
        var projectedMember = await TestMemberRepository.GetMemberById(existingMember.Id);
        
        Assert.That(projectedMember, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectedMember.Id, Is.EqualTo(existingMember.Id));
            Assert.That(projectedMember.Club.Id, Is.EqualTo(Guid.Parse("0411624f-ac2a-4ce8-a02f-b8978bb6d84d")));
            Assert.That(projectedMember.IsLocked, Is.False);
        });
    }
    
    
}