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
    public class ClubServiceTests
    {
        private Mock<IRepository<Club>> mockBaseRepository;
        private Mock<IClubRepository> mockClubRepository;
        private Mock<IRepository<GameSession>> mockGameSessionRepository;
        private ClubService service;

        [SetUp]
        public void SetUp()
        {
            mockBaseRepository = new Mock<IRepository<Club>>();
            mockClubRepository = new Mock<IClubRepository>();
            mockGameSessionRepository = new Mock<IRepository<GameSession>>();

            service = new ClubService(mockBaseRepository.Object, mockClubRepository.Object, mockGameSessionRepository.Object);
        }

        [Test]
        public async Task GetAllActiveClubs_ReturnsOnlyNotDeletedClubs()
        {
            var clubs = new List<Club>
            {
                new Club { Id = Guid.NewGuid(), Name = "Club1", IsDeleted = false, City = new City { Id = Guid.NewGuid(), Name = "City1" } },
                new Club { Id = Guid.NewGuid(), Name = "Club2", IsDeleted = true, City = new City { Id = Guid.NewGuid(), Name = "City2" } },
                new Club { Id = Guid.NewGuid(), Name = "Club3", IsDeleted = false, City = new City { Id = Guid.NewGuid(), Name = "City3" } },
            }.BuildMock();

            mockClubRepository.Setup(r => r.All()).Returns(clubs);

            var result = await service.GetAllActiveClubs();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(c => c.ClubName == "Club1" || c.ClubName == "Club3"));
        }

        [Test]
        public async Task GetClubDetailsAsync_ReturnsNull_WhenClubNotFound()
        {
            var id = Guid.NewGuid();
            mockClubRepository.Setup(r => r.GetDetailsByIdAsync(id)).ReturnsAsync((Club)null);

            var result = await service.GetClubDetailsAsync(id);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetClubDetailsAsync_ReturnsCorrectData()
        {
            var id = Guid.NewGuid();

            var club = new Club
            {
                Id = id,
                Name = "Test Club",
                Address = "Some Address",
                City = new City { Id = Guid.NewGuid(), Name = "Test City" },
                ClubBoardGames = new List<ClubBoardGame>
                {
                    new ClubBoardGame
                    {
                        BoardGame = new BoardGame
                        {
                            Id = Guid.NewGuid(),
                            Title = "Game 1",
                            ImageUrl = "url1",
                            GameSessions = new List<GameSession>
                            {
                                new GameSession
                                {
                                    Id = Guid.NewGuid(),
                                    ClubId = id,
                                    IsDeleted = false,
                                    StartTime = DateTime.UtcNow
                                }
                            }
                        }
                    }
                }
            };

            mockClubRepository.Setup(r => r.GetDetailsByIdAsync(id)).ReturnsAsync(club);

            var result = await service.GetClubDetailsAsync(id);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(club.Id));
            Assert.That(result.Name, Is.EqualTo(club.Name));
            Assert.That(result.CityName, Is.EqualTo(club.City.Name));
            Assert.That(result.BoardGames.Count(), Is.EqualTo(1));
            Assert.That(result.BoardGames.First().Title, Is.EqualTo("Game 1"));
            Assert.That(result.BoardGames.First().ActiveGameSessionId, Is.EqualTo(club.ClubBoardGames.First().BoardGame.GameSessions.First().Id));
        }
    }
}
