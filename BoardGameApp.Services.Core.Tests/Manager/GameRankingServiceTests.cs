namespace BoardGameApp.Services.Core.Tests.Manager
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Manager;
    using BoardGameApp.Web.ViewModels.Manager.GameRanking;
    using Moq;
    using MockQueryable;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class GameRankingServiceTests
    {
        private Mock<IRepository<GameRanking>> mockGameRankingRepo = null!;
        private GameRankingService service = null!;

        [SetUp]
        public void Setup()
        {
            mockGameRankingRepo = new Mock<IRepository<GameRanking>>();
            service = new GameRankingService(mockGameRankingRepo.Object);
        }

        [Test]
        public async Task GetAllGameRankingsAsync_ReturnsAllRankings()
        {
            var data = new List<GameRanking>
            {
                new GameRanking
                {
                    Id = Guid.NewGuid(),
                    Wins = 5,
                    Losses = 3,
                    Draws = 2,
                    LastUpdated = DateTime.UtcNow,
                    User = new BoardgameUser { UserName = "Player1" },
                    BoardGame = new BoardGame { Title = "Chess" }
                },
                new GameRanking
                {
                    Id = Guid.NewGuid(),
                    Wins = 7,
                    Losses = 1,
                    Draws = 1,
                    LastUpdated = DateTime.UtcNow,
                    User = new BoardgameUser { UserName = "Player2" },
                    BoardGame = new BoardGame { Title = "Monopoly" }
                }
            }.BuildMock();            

            mockGameRankingRepo.Setup(r => r.All()).Returns(data);

            var result = await service.GetAllGameRankingsAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(r => r.Username == "Player1" && r.BoardGameTitle == "Chess"));
            Assert.That(result.Any(r => r.Username == "Player2" && r.BoardGameTitle == "Monopoly"));
        }

        [Test]
        public async Task UpdateAsync_WithValidId_UpdatesRanking()
        {
            var existingRanking = new GameRanking
            {
                Id = Guid.NewGuid(),
                Wins = 1,
                Losses = 1,
                Draws = 1
            };

            mockGameRankingRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<GameRanking, bool>>>()))
                .ReturnsAsync(existingRanking);

            mockGameRankingRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var updateModel = new GameRankingBaseModel
            {
                Id = existingRanking.Id,
                Wins = 10,
                Losses = 5,
                Draws = 3
            };

            await service.UpdateAsync(updateModel);

            Assert.That(existingRanking.Wins, Is.EqualTo(10));
            Assert.That(existingRanking.Losses, Is.EqualTo(5));
            Assert.That(existingRanking.Draws, Is.EqualTo(3));

            mockGameRankingRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateAsync_WithInvalidId_ThrowsArgumentException()
        {
            mockGameRankingRepo
                .Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<GameRanking, bool>>>()))
                .ReturnsAsync((GameRanking?)null);

            var updateModel = new GameRankingBaseModel
            {
                Id = Guid.NewGuid(),
                Wins = 0,
                Losses = 0,
                Draws = 0
            };

            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(updateModel));
        }
    }
}
