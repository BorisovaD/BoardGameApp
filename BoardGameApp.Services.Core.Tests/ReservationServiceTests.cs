namespace BoardGameApp.Services.Core.Tests
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using Moq;
    using MockQueryable;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class ReservationServiceTests
    {
        private Mock<IRepository<Reservation>> mockReservationRepo;
        private ReservationService service;

        [SetUp]
        public void SetUp()
        {
            mockReservationRepo = new Mock<IRepository<Reservation>>();
            service = new ReservationService(mockReservationRepo.Object);
        }

        [Test]
        public async Task GetAllActiveReservations_ReturnsCorrectData()
        {
            var boardGame = new BoardGame { Id = Guid.NewGuid(), Title = "Catan", ImageUrl = "image.jpg" };
            var gameSession = new GameSession { Id = Guid.NewGuid(), BoardGame = boardGame, StartTime = DateTime.UtcNow };
            var reservationId = Guid.NewGuid();
            var user = new BoardgameUser
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Tickets = new List<Ticket>
                {
                    new Ticket { Id = Guid.NewGuid(), ReservationId = reservationId, Quantity = 2 },
                    new Ticket { Id = Guid.NewGuid(), ReservationId = reservationId, Quantity = 1 }
                }
            };

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    Id = reservationId,
                    ReservationTime = DateTime.UtcNow,
                    User = user,
                    UserId = user.Id,
                    GameSession = gameSession,
                    GameSessionId = gameSession.Id
                }
            }.BuildMock();

            mockReservationRepo
                .Setup(r => r.All())
                .Returns(reservations);

            var result = (await service.GetAllActiveReservations()).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            var res = result[0];
            Assert.That(res.UserEmail, Is.EqualTo("test@example.com"));
            Assert.That(res.BoardGameTitle, Is.EqualTo("Catan"));
            Assert.That(res.Tickets, Is.EqualTo(3));
            Assert.That(res.ImageUrl, Is.EqualTo("image.jpg"));
        }

        [Test]
        public async Task GetMyReservationsAsync_ReturnsOnlyUserReservations()
        {            
            var userId = Guid.NewGuid();
            var reservationId = Guid.NewGuid();
            var boardGame = new BoardGame { Id = Guid.NewGuid(), Title = "Azul", ImageUrl = "azul.jpg" };
            var gameSession = new GameSession { Id = Guid.NewGuid(), BoardGame = boardGame, StartTime = DateTime.UtcNow };

            var tickets = new List<Ticket>
            {
                new Ticket { Id = Guid.NewGuid(), ReservationId = reservationId, Quantity = 4 }
            };

            var user = new BoardgameUser
            {
                Id = userId,
                Email = "user@abv.bg",
                Tickets = tickets
            };

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    Id = reservationId,
                    UserId = userId,
                    User = user,
                    GameSession = gameSession,
                    GameSessionId = gameSession.Id,
                    ReservationTime = DateTime.UtcNow
                },
                
                new Reservation
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    User = new BoardgameUser { Email = "someone@else.bg" },
                    GameSession = gameSession,
                    GameSessionId = gameSession.Id,
                    ReservationTime = DateTime.UtcNow
                }
            }.BuildMock();

            mockReservationRepo
                .Setup(r => r.All())
                .Returns(reservations);

            var result = (await service.GetMyReservationsAsync(userId)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            var res = result[0];
            Assert.That(res.UserEmail, Is.EqualTo("user@abv.bg"));
            Assert.That(res.Tickets, Is.EqualTo(4));
            Assert.That(res.BoardGameTitle, Is.EqualTo("Azul"));
        }
    }
}
