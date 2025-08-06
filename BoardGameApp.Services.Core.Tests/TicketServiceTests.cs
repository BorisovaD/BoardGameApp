namespace BoardGameApp.Services.Core.Tests
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Contracts;
    using Moq;
    using MockQueryable;
    using MockQueryable.Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class TicketServiceTests
    {
        private Mock<IRepository<Ticket>> ticketRepoMock;
        private Mock<IRepository<GameSession>> sessionRepoMock;
        private Mock<IRepository<Reservation>> reservationRepoMock;
        private Mock<IRepository<GameRanking>> rankingRepoMock;
        private ITicketService ticketService;

        [SetUp]
        public void SetUp()
        {
            ticketRepoMock = new Mock<IRepository<Ticket>>();
            sessionRepoMock = new Mock<IRepository<GameSession>>();
            reservationRepoMock = new Mock<IRepository<Reservation>>();
            rankingRepoMock = new Mock<IRepository<GameRanking>>();

            ticketService = new TicketService(
                ticketRepoMock.Object,
                sessionRepoMock.Object,
                reservationRepoMock.Object,
                rankingRepoMock.Object
            );
        }

        [Test]
        public async Task BuyTicketAsync_ShouldReturnFalse_WhenSessionDoesNotExist()
        {
            var sessionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            sessionRepoMock.Setup(r => r.All())
                .Returns(new List<GameSession>().BuildMock());

            var result = await ticketService.BuyTicketAsync(userId, sessionId, 1);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task BuyTicketAsync_ShouldReturnFalse_WhenNotEnoughTickets()
        {
            var sessionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var session = new GameSession
            {
                Id = sessionId,
                MaxPlayers = 5,
                Reservations = new List<Reservation>
                {
                    new Reservation { Id = Guid.NewGuid() }
                }
            };

            sessionRepoMock.Setup(r => r.All())
                .Returns(new List<GameSession> { session }.BuildMock());

            ticketRepoMock.Setup(r => r.All())
                .Returns(new List<Ticket>
                {
                    new Ticket { ReservationId = session.Reservations.First().Id, Quantity = 5 }
                }.BuildMock());

            var result = await ticketService.BuyTicketAsync(userId, sessionId, 1);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task BuyTicketAsync_ShouldBuyTicket_WhenUserHasReservation()
        {
            var sessionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var reservationId = Guid.NewGuid();

            var reservation = new Reservation
            {
                Id = reservationId,
                UserId = userId,
                GameSessionId = sessionId
            };

            var session = new GameSession
            {
                Id = sessionId,
                MaxPlayers = 10,
                Reservations = new List<Reservation> { reservation }
            };

            sessionRepoMock.Setup(r => r.All())
                .Returns(new List<GameSession> { session }.BuildMock());

            ticketRepoMock.Setup(r => r.All())
                .Returns(new List<Ticket>().BuildMock());

            var result = await ticketService.BuyTicketAsync(userId, sessionId, 2);

            Assert.IsTrue(result);
            ticketRepoMock.Verify(r => r.AddAsync(It.IsAny<Ticket>()), Times.Once);
            ticketRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task BuyTicketAsync_ShouldCreateReservation_WhenUserHasNone()
        {
            var sessionId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var session = new GameSession
            {
                Id = sessionId,
                MaxPlayers = 10,
                Reservations = new List<Reservation>()
            };

            sessionRepoMock.Setup(r => r.All())
                .Returns(new List<GameSession> { session }.BuildMock());

            ticketRepoMock.Setup(r => r.All())
                .Returns(new List<Ticket>().BuildMock());

            var result = await ticketService.BuyTicketAsync(userId, sessionId, 2);

            Assert.IsTrue(result);
            reservationRepoMock.Verify(r => r.AddAsync(It.IsAny<Reservation>()), Times.Once);
            reservationRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            ticketRepoMock.Verify(r => r.AddAsync(It.IsAny<Ticket>()), Times.Once);
            ticketRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllTickets_ShouldReturnCorrectData()
        {
            var ticketId = Guid.NewGuid();
            var reservationId = Guid.NewGuid();
            var gameSessionId = Guid.NewGuid();
            var boardGame = new BoardGame { Title = "Catan" };
            var gameSession = new GameSession { Id = gameSessionId, BoardGame = boardGame };
            var reservation = new Reservation { Id = reservationId, GameSession = gameSession };
            var user = new BoardgameUser { UserName = "DesiGamer" };

            var ticket = new Ticket
            {
                Id = ticketId,
                IssuedOn = new DateTime(2025, 8, 6),
                Price = 15.00m,
                Quantity = 2,
                Reservation = reservation,
                ReservationId = reservationId,
                User = user
            };

            var tickets = new List<Ticket> { ticket };

            ticketRepoMock.Setup(r => r.All())
                .Returns(tickets.BuildMock());

            var result = (await ticketService.GetAllTickets()).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            var returned = result[0];
            Assert.That(returned.Id, Is.EqualTo(ticketId));
            Assert.That(returned.IssuedOn, Is.EqualTo(ticket.IssuedOn));
            Assert.That(returned.Price, Is.EqualTo(15.00m));
            Assert.That(returned.Quantity, Is.EqualTo(2));
            Assert.That(returned.ReservationId, Is.EqualTo(reservationId));
            Assert.That(returned.BoardGameTitle, Is.EqualTo("Catan"));
            Assert.That(returned.UserName, Is.EqualTo("DesiGamer"));
        }

        [Test]
        public async Task GetTicketDetailsAsync_ShouldReturnCorrectDetails()
        {
            var ticketId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var gameSessionId = Guid.NewGuid();
            var boardGame = new BoardGame { Title = "Codenames" };
            var gameSession = new GameSession
            {
                Id = gameSessionId,
                StartTime = new DateTime(2025, 8, 10, 17, 0, 0),
                EndTime = new DateTime(2025, 8, 10, 19, 0, 0),
                BoardGame = boardGame
            };
            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                ReservationTime = new DateTime(2025, 8, 6, 10, 0, 0),
                GameSession = gameSession,
                GameSessionId = gameSessionId
            };
            var user = new BoardgameUser { Id = userId, UserName = "DesiHero" };

            var ticket = new Ticket
            {
                Id = ticketId,
                IssuedOn = new DateTime(2025, 8, 6, 11, 0, 0),
                Price = 12.00m,
                Quantity = 3,
                Reservation = reservation,
                ReservationId = reservation.Id,
                User = user
            };

            var tickets = new List<Ticket> { ticket };

            ticketRepoMock.Setup(r => r.All())
                .Returns(tickets.BuildMock());

            var result = await ticketService.GetTicketDetailsAsync(ticketId, userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(ticketId));
            Assert.That(result.IssuedOn, Is.EqualTo(ticket.IssuedOn));
            Assert.That(result.Price, Is.EqualTo(12.00m));
            Assert.That(result.Quantity, Is.EqualTo(3));
            Assert.That(result.ReservationId, Is.EqualTo(reservation.Id));
            Assert.That(result.ReservationTime, Is.EqualTo(reservation.ReservationTime));
            Assert.That(result.GameSessionId, Is.EqualTo(gameSessionId));
            Assert.That(result.StartTime, Is.EqualTo(gameSession.StartTime));
            Assert.That(result.EndTime, Is.EqualTo(gameSession.EndTime));
            Assert.That(result.BoardGameTitle, Is.EqualTo("Codenames"));
            Assert.That(result.UserName, Is.EqualTo("DesiHero"));
        }

        [Test]
        public async Task GetTicketDetailsAsync_ShouldReturnNull_WhenIdIsNull()
        {
            var result = await ticketService.GetTicketDetailsAsync(null, Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetTicketDetailsAsync_ShouldReturnNull_WhenTicketDoesNotExist()
        {
            var nonExistentId = Guid.NewGuid();

            var tickets = new List<Ticket>(); 

            ticketRepoMock.Setup(r => r.All())
                .Returns(tickets.BuildMock());

            var result = await ticketService.GetTicketDetailsAsync(nonExistentId, Guid.NewGuid());

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetTicketInfoAsync_ShouldReturnCorrectModel_WhenGameSessionExists()
        {
            var gameSessionId = Guid.NewGuid();
            var boardGame = new BoardGame { Title = "Catan" };
            var gameSession = new GameSession
            {
                Id = gameSessionId,
                BoardGame = boardGame,
                MaxPlayers = 10,
                IsDeleted = false
            };

            var reservations = new List<Reservation>
            {
                new Reservation { Id = Guid.NewGuid(), GameSessionId = gameSessionId },
                new Reservation { Id = Guid.NewGuid(), GameSessionId = gameSessionId }
            };

            var tickets = new List<Ticket>
            {
                new Ticket { Quantity = 3, ReservationId = reservations[0].Id },
                new Ticket { Quantity = 2, ReservationId = reservations[1].Id }
            };

            reservationRepoMock.Setup(r => r.All())
                .Returns(reservations.BuildMock());

            ticketRepoMock.Setup(t => t.All())
                .Returns(tickets.BuildMock());

            sessionRepoMock.Setup(g => g.All())
                .Returns(new List<GameSession> { gameSession }.BuildMock());

            var result = await ticketService.GetTicketInfoAsync(gameSessionId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.GameSessionId, Is.EqualTo(gameSessionId));
            Assert.That(result.GameTitle, Is.EqualTo("Catan"));
            Assert.That(result.AvailableTickets, Is.EqualTo(5)); 
        }

        [Test]
        public async Task GetTicketInfoAsync_ShouldReturnNull_WhenGameSessionIsDeletedOrNotFound()
        {
            var sessionId = Guid.NewGuid();

            sessionRepoMock.Setup(g => g.All())
                .Returns(new List<GameSession>  
                {
                    new GameSession { Id = Guid.NewGuid(), IsDeleted = true }
                }.BuildMock());

            reservationRepoMock.Setup(r => r.All())
                .Returns(new List<Reservation>().BuildMock());

            ticketRepoMock.Setup(t => t.All())
                .Returns(new List<Ticket>().BuildMock());

            var result = await ticketService.GetTicketInfoAsync(sessionId);

            Assert.That(result, Is.Null);
        }
    }
}
