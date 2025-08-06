namespace BoardGameApp.Services.Core.Tests
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using MockQueryable;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class GameSessionServiceTests
    {
        private Mock<IRepository<GameSession>> mockGameSessionRepository;
        private GameSessionService service;

        [SetUp]
        public void Setup()
        {
            mockGameSessionRepository = new Mock<IRepository<GameSession>>();
            service = new GameSessionService(mockGameSessionRepository.Object);
        }

        [Test]
        public async Task GetAllActiveGameSessions_ReturnsOnlyNotDeletedSessions()
        {
            var clubCity = new City { Id = Guid.NewGuid(), Name = "Sofia" };
            var club = new Club { Id = Guid.NewGuid(), Name = "Boardgame Club", City = clubCity };
            var boardGame = new BoardGame { Id = Guid.NewGuid(), Title = "Catan", ImageUrl = "image.jpg" };

            var sessions = new List<GameSession>
            {
                new GameSession
                {
                    Id = Guid.NewGuid(),
                    BoardGame = boardGame,
                    Club = club,
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddHours(2),
                    MaxPlayers = 5,
                    CurrentPlayers = 3,
                    IsDeleted = false
                },
                new GameSession
                {
                    Id = Guid.NewGuid(),
                    BoardGame = boardGame,
                    Club = club,
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddHours(1),
                    MaxPlayers = 4,
                    CurrentPlayers = 2,
                    IsDeleted = true
                }
            }.BuildMock();

            mockGameSessionRepository.Setup(r => r.All()).Returns(sessions);

            var result = (await service.GetAllActiveGameSessions()).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            var first = result[0];
            Assert.That(first.BoardGameName, Is.EqualTo("Catan"));
            Assert.That(first.ClubName, Is.EqualTo("Boardgame Club"));
            Assert.That(first.CityName, Is.EqualTo("Sofia"));
            Assert.That(first.MaxPlayers, Is.EqualTo(5));
            Assert.That(first.CurrentPlayers, Is.EqualTo(3));
            Assert.That(first.ImageUrl, Is.EqualTo("image.jpg"));
        }
    }
}
