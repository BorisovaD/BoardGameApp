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
    public class RankingServiceTests
    {
        private Mock<IRepository<GameRanking>> mockRepository;
        private RankingService rankingService;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new Mock<IRepository<GameRanking>>();
            rankingService = new RankingService(mockRepository.Object);
        }

        [Test]
        public async Task GetAllRankingsAsync_ReturnsCorrectRankingViewModels()
        {
            var gameRankings = new List<GameRanking>
        {
            new GameRanking
            {
                Id = Guid.NewGuid(),
                Wins = 5,
                Losses = 2,
                Draws = 1,
                User = new BoardgameUser { UserName = "User1" },
                BoardGame = new BoardGame { Title = "Game1" }
            },
            new GameRanking
            {
                Id = Guid.NewGuid(),
                Wins = 3,
                Losses = 4,
                Draws = 0,
                User = new BoardgameUser { UserName = "User2" },
                BoardGame = new BoardGame { Title = "Game2" }
            }
        }.BuildMock();

            mockRepository.Setup(r => r.All()).Returns(gameRankings);

            var result = await rankingService.GetAllRankingsAsync();

            Assert.IsNotNull(result);
            var rankingsList = result.ToList();
            Assert.That(rankingsList.Count, Is.EqualTo(2));

            Assert.That(rankingsList[0].Username, Is.EqualTo("User1"));
            Assert.That(rankingsList[0].BoardGameTitle, Is.EqualTo("Game1"));
            Assert.That(rankingsList[0].Wins, Is.EqualTo(5));

            Assert.That(rankingsList[1].Username, Is.EqualTo("User2"));
            Assert.That(rankingsList[1].BoardGameTitle, Is.EqualTo("Game2"));
            Assert.That(rankingsList[1].Wins, Is.EqualTo(3));
        }
    }
}
