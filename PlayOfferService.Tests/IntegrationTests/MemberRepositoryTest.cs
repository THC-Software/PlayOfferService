using NSubstitute;
using NUnit.Framework.Internal;
using PlayOfferService.Domain.Events;
using PlayOfferService.Domain.Events.Member;
using PlayOfferService.Domain.Repositories;
using PlayOfferService.Models;

namespace PlayOfferService.Tests.IntegrationTests;

[TestFixture]
public class MemberRepositoryTest : TestSetup
{
    private ClubRepository _clubRepositoryMock;
    
    [SetUp]
    public async Task MemberSetup()
    {
        var testClub = new Club { Id = Guid.NewGuid(), IsLocked = false };
        _clubRepositoryMock = Substitute.For<ClubRepository>();
        _clubRepositoryMock.GetClubById(Guid.NewGuid()).ReturnsForAnyArgs(testClub);
        
        var memberCreationEvent = new BaseEvent
        {
            EntityId = Guid.Parse("16a1a213-f684-4e08-b9ef-61b372c59bf4"),
            EntityType = EntityType.MEMBER,
            EventId = Guid.NewGuid(),
            EventType = EventType.MEMBER_ACCOUNT_CREATED,
            EventData = new MemberCreatedEvent
            {
                MemberAccountId = Guid.Parse("16a1a213-f684-4e08-b9ef-61b372c59bf4"),
                Club = testClub
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
                Club = testClub
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
        var testClub = await _clubRepositoryMock.GetClubById(Guid.NewGuid());
        
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
                Club = testClub
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
            Assert.That(projectedMember.Club.Id, Is.EqualTo(testClub.Id));
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
            Assert.That(projectedMember.IsLocked, Is.False);
        });
    }
}