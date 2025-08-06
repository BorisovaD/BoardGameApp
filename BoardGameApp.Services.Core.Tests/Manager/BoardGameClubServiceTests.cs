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
    public class BoardGameClubServiceTests
    {
        private Mock<IRepository<ClubBoardGame>> mockRepo;
        private BoardGameClubService service;

        [SetUp]
        public void SetUp()
        {
            mockRepo = new Mock<IRepository<ClubBoardGame>>();
            service = new BoardGameClubService(mockRepo.Object);
        }

        [Test]
        public async Task ToggleGameInClubAsync_RemovesEntry_WhenItExists()
        {
            var clubId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            var existingEntry = new ClubBoardGame
            {
                ClubId = clubId,
                BoardGameId = gameId
            };

            var data = new List<ClubBoardGame> { existingEntry }.AsQueryable();

            mockRepo.Setup(r => r.All()).Returns(data);
            mockRepo.Setup(r => r.Delete(existingEntry));
            mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await service.ToggleGameInClubAsync(clubId, gameId);

            mockRepo.Verify(r => r.Delete(existingEntry), Times.Once);
            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<ClubBoardGame>()), Times.Never);
        }

        [Test]
        public async Task ToggleGameInClubAsync_AddsEntry_WhenItDoesNotExist()
        {
            var clubId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            var data = new List<ClubBoardGame>().AsQueryable();

            mockRepo.Setup(r => r.All()).Returns(data);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<ClubBoardGame>())).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await service.ToggleGameInClubAsync(clubId, gameId);

            mockRepo.Verify(r => r.AddAsync(It.Is<ClubBoardGame>(e =>
                e.ClubId == clubId && e.BoardGameId == gameId)), Times.Once);

            mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            mockRepo.Verify(r => r.Delete(It.IsAny<ClubBoardGame>()), Times.Never);
        }
    }
}
