namespace BoardGameApp.Services.Core.Tests.Manager
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Manager;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class TicketReservationServiceTests
    {
        private Mock<IRepository<GameSession>> mockGameSessionRepo = null!;
        private TicketReservationService service = null!;

        [SetUp]
        public void Setup()
        {
            mockGameSessionRepo = new Mock<IRepository<GameSession>>();
            service = new TicketReservationService(mockGameSessionRepo.Object);
        }

        [Test]
        public async Task UpdateMaxPlayersAsync_WithValidSession_UpdatesAndReturnsTrue()
        {
            var sessionId = Guid.NewGuid();
            var session = new GameSession
            {
                Id = sessionId,
                IsDeleted = false,
                CurrentPlayers = 0,
                MaxPlayers = 4
            };

            mockGameSessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(session);
            mockGameSessionRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await service.UpdateMaxPlayersAsync(sessionId, 6);

            Assert.IsTrue(result);
            Assert.That(session.MaxPlayers, Is.EqualTo(6));
            mockGameSessionRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateMaxPlayersAsync_WithInvalidSession_ReturnsFalse()
        {
            var sessionId = Guid.NewGuid();

            mockGameSessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync((GameSession?)null);

            var resultNull = await service.UpdateMaxPlayersAsync(sessionId, 6);
            Assert.IsFalse(resultNull);

            var deletedSession = new GameSession { Id = sessionId, IsDeleted = true, CurrentPlayers = 0 };
            mockGameSessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(deletedSession);

            var resultDeleted = await service.UpdateMaxPlayersAsync(sessionId, 6);
            Assert.IsFalse(resultDeleted);

            var busySession = new GameSession { Id = sessionId, IsDeleted = false, CurrentPlayers = 2 };
            mockGameSessionRepo.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync(busySession);

            var resultBusy = await service.UpdateMaxPlayersAsync(sessionId, 6);
            Assert.IsFalse(resultBusy);

            mockGameSessionRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
