using PlayOfferService.Domain.Models;

namespace PlayOfferService.Tests.IntegrationTests;

public class ReservationRepositoryTest : TestSetup
{
    [SetUp]
    public async Task ReservationSetup()
    {
        var existingReservation = new Reservation
        {
            Id = Guid.Parse("a3f6b4bc-b3cd-49da-9663-3cdb9c9a60bb"),
            CourtIds = [Guid.Parse("cc02023d-9306-471c-b298-8bc584e8ef8a")],
            ReservantId = Guid.Parse("40337148-538c-44b5-b08a-d3a2cd4ddd13"),
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            ParticipantsIds = [Guid.Parse("40337148-538c-44b5-b08a-d3a2cd4ddd13"), Guid.Parse("20f2e28c-db33-4962-93ce-218afd311fca")],
            IsCancelled = false
        };
        TestReservationRepository.CreateReservation(existingReservation);
        await TestReservationRepository.Update();
    }
    
    [Test]
    public async Task GetExistingReservationByIdTest()
    {
        //Given
        var reservationId = Guid.Parse("a3f6b4bc-b3cd-49da-9663-3cdb9c9a60bb");
        
        //When
        var reservation = await TestReservationRepository.GetReservationById(reservationId);
        
        //Then
        Assert.That(reservation, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(reservation!.Id, Is.EqualTo(reservationId));
            Assert.That(reservation.CourtIds, Is.EqualTo(new List<Guid> {Guid.Parse("cc02023d-9306-471c-b298-8bc584e8ef8a")}));
            Assert.That(reservation.ReservantId, Is.EqualTo(Guid.Parse("40337148-538c-44b5-b08a-d3a2cd4ddd13")));
            Assert.That(reservation.StartTime, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
            Assert.That(reservation.EndTime, Is.EqualTo(DateTime.UtcNow.AddHours(1)).Within(1).Seconds);
            Assert.That(reservation.ParticipantsIds, Is.EqualTo(new List<Guid> {Guid.Parse("40337148-538c-44b5-b08a-d3a2cd4ddd13"), Guid.Parse("20f2e28c-db33-4962-93ce-218afd311fca")}));
            Assert.That(reservation.IsCancelled, Is.False);
        });
    }
    
    [Test]
    public async Task GetNonExistingReservationByIdTest()
    {
        //Given
        var reservationId = Guid.NewGuid();
        
        //When
        var reservation = await TestReservationRepository.GetReservationById(reservationId);
        
        //Then
        Assert.That(reservation, Is.Null);
    }
    
    [Test]
    public async Task CreateReservationTest()
    {
        //Given
        var givenReservation = new Reservation
        {
            Id = Guid.Parse("8ef448d9-5099-4226-8d12-582165c4f5e9"),
            CourtIds = [Guid.Parse("cc02023d-9306-471c-b298-8bc584e8ef8a")],
            ReservantId = Guid.Parse("20f2e28c-db33-4962-93ce-218afd311fca"),
            ParticipantsIds = [Guid.Parse("20f2e28c-db33-4962-93ce-218afd311fca"),Guid.Parse("40337148-538c-44b5-b08a-d3a2cd4ddd13")],
            IsCancelled = false
        };
        
        //When
        TestReservationRepository.CreateReservation(givenReservation);
        await TestReservationRepository.Update();
        
        //Then
        var reservation = await TestReservationRepository.GetReservationById(givenReservation.Id);
        
        Assert.That(reservation, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(reservation!.Id, Is.EqualTo(Guid.Parse("8ef448d9-5099-4226-8d12-582165c4f5e9")));
            Assert.That(reservation.CourtIds, Is.EqualTo(new List<Guid> {Guid.Parse("cc02023d-9306-471c-b298-8bc584e8ef8a")}));
            Assert.That(reservation.ReservantId, Is.EqualTo(Guid.Parse("20f2e28c-db33-4962-93ce-218afd311fca")));
            Assert.That(reservation.ParticipantsIds, Is.EqualTo(new List<Guid> {Guid.Parse("20f2e28c-db33-4962-93ce-218afd311fca"),Guid.Parse("40337148-538c-44b5-b08a-d3a2cd4ddd13")}));
            Assert.That(reservation.IsCancelled, Is.False);
        });
    }
}