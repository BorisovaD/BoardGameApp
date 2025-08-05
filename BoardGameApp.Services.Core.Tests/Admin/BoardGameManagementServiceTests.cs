namespace BoardGameApp.Services.Core.Tests.Admin
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Admin;
    using Moq;
    using MockQueryable;
    using Moq.EntityFrameworkCore;

    [TestFixture]
    public class BoardGameManagementServiceTests
    {
        private Mock<IRepository<BoardGame>> boardGameRepositoryMock;
        private BoardGameManagementService boardGameManagementService;

        [SetUp]
        public void Setup()
        {
            this.boardGameRepositoryMock = new Mock<IRepository<BoardGame>>(MockBehavior.Strict);
            this.boardGameManagementService = new BoardGameManagementService(this.boardGameRepositoryMock.Object);
        }

        [Test]
        public void PassAlways()
        {
            Assert.Pass();
        }

        [Test]
        public async Task GetBoardGamesManagementInfoAsync_ShouldReturnCorrectData()
        {            
            var testGames = new List<BoardGame>
            {
                new BoardGame
                {
                    Id = Guid.Parse("162ab95b-e2cb-4a08-8e51-651514eb3178"),
                    Title = "Alpha Game",
                    Duration = 30,
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    IsDeleted = false,
                    BoardGameCategories = new List<BoardGameCategory>
                    {
                        new BoardGameCategory
                        {
                            Category = new Category { Name = "Strategy" }
                        }
                    }
                },
                new BoardGame
                {
                    Id = Guid.Parse("f6ad96c2-e58e-4a4d-9761-10d3bbd2c9e8"),
                    Title = "Beta Game",
                    Duration = 45,
                    MinPlayers = 1,
                    MaxPlayers = 5,
                    IsDeleted = true,
                    BoardGameCategories = new List<BoardGameCategory>
                    {
                        new BoardGameCategory
                        {
                            Category = new Category { Name = "Family" }
                        },
                        new BoardGameCategory
                        {
                            Category = new Category { Name = "Fun" }
                        }
                    }
                }
            };

            var testGamesQueryable = testGames.BuildMock();

            this.boardGameRepositoryMock
                .Setup(r => r.All())
                .Returns(testGamesQueryable);

            var result = await this.boardGameManagementService.GetBoardGamesManagementInfoAsync();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));

            var firstGame = result.First();
            Assert.That(firstGame.Title, Is.EqualTo("Alpha Game"));
            Assert.That(firstGame.Duration, Is.EqualTo(30));
            Assert.That(firstGame.MinPlayers, Is.EqualTo(2));
            Assert.That(firstGame.MaxPlayers, Is.EqualTo(4));
            Assert.IsFalse(firstGame.IsDeleted);
            Assert.That(firstGame.Categories, Does.Contain("Strategy"));

            var secondGame = result.Skip(1).First();
            Assert.That(secondGame.Title, Is.EqualTo("Beta Game"));
            Assert.IsTrue(secondGame.IsDeleted);
            Assert.That(secondGame.Categories, Does.Contain("Family"));
            Assert.That(secondGame.Categories, Does.Contain("Fun"));
        }

        [Test]
        public async Task GetBoardGamesManagementInfoAsync_ShouldReturnEmptyList_WhenNoGames()
        {
            this.boardGameRepositoryMock
                .Setup(r => r.All())
                .Returns(new List<BoardGame>().BuildMock());

            var result = await this.boardGameManagementService.GetBoardGamesManagementInfoAsync();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
    }
}

