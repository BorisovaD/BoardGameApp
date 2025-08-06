namespace BoardGameApp.Services.Core.Tests.Manager
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Manager;
    using Moq;
    using MockQueryable;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class CatalogServiceTests
    {
        private Mock<IRepository<BoardGame>> mockBoardGameRepo;
        private Mock<IRepository<ClubBoardGame>> mockClubBoardGameRepo;
        private Mock<IRepository<Manager>> mockManagerRepo;

        private CatalogService catalogService;

        [SetUp]
        public void Setup()
        {
            mockBoardGameRepo = new Mock<IRepository<BoardGame>>();
            mockClubBoardGameRepo = new Mock<IRepository<ClubBoardGame>>();
            mockManagerRepo = new Mock<IRepository<Manager>>();

            catalogService = new CatalogService(
                mockBoardGameRepo.Object,
                mockClubBoardGameRepo.Object,
                mockManagerRepo.Object);
        }

        [Test]
        public async Task GetAllBoardGamesAsync_ReturnsGamesWithCorrectClubFlag()
        {
            var clubId = Guid.NewGuid();

            var games = new List<BoardGame>
            {
                new BoardGame
                {
                    Id = Guid.NewGuid(),
                    Title = "Game 1",
                    IsDeleted = false,
                    ImageUrl = "url1",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    Duration = 60,
                    ClubBoardGames = new List<ClubBoardGame>
                    {
                        new ClubBoardGame { ClubId = clubId }
                    }
                },
                new BoardGame
                {
                    Id = Guid.NewGuid(),
                    Title = "Game 2",
                    IsDeleted = false,
                    ImageUrl = "url2",
                    MinPlayers = 1,
                    MaxPlayers = 5,
                    Duration = 45,
                    ClubBoardGames = new List<ClubBoardGame>() 
                }
            }.BuildMock();

            mockBoardGameRepo.Setup(r => r.All()).Returns(games);

            var result = await catalogService.GetAllBoardGamesAsync(clubId);

            Assert.That(result.Count(), Is.EqualTo(2));

            var game1 = result.First(g => g.Title == "Game 1");
            var game2 = result.First(g => g.Title == "Game 2");

            Assert.That(game1.IsInClub, Is.True);
            Assert.That(game2.IsInClub, Is.False);
        }

        [Test]
        public async Task GetClubIdByManagerIdAsync_ReturnsCorrectClubId()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            var clubId = Guid.NewGuid();

            var managers = new List<Manager>
            {
                new Manager { UserId = managerId, ClubId = clubId }
            }.BuildMock();

            mockManagerRepo.Setup(r => r.All()).Returns(managers);

            var result = await catalogService.GetClubIdByManagerIdAsync(managerId);

            Assert.That(result, Is.EqualTo(clubId));
        }

        [Test]
        public async Task GetClubIdByManagerIdAsync_ReturnsNull_WhenManagerNotFound()
        {
            var managerId = Guid.NewGuid();

            var managers = new List<Manager>().BuildMock();

            mockManagerRepo.Setup(r => r.All()).Returns(managers);

            var result = await catalogService.GetClubIdByManagerIdAsync(managerId);

            Assert.That(result, Is.Null);
        }
    }
}
