namespace BoardGameApp.Services.Core.Tests.Manager
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Manager;
    using BoardGameApp.Web.ViewModels.Manager.GameSessions;
    using Moq;
    using MockQueryable.Moq;
    using Moq.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MockQueryable;
    using BoardGameApp.Services.Core.Admin;

    [TestFixture]
    public class BoardGameSessionsServiceTests
    {
        private Mock<IRepository<GameSession>> mockGameSessionRepo;
        private Mock<IRepository<ClubBoardGame>> mockClubBoardGameRepo;
        private Mock<IRepository<BoardGame>> mockBoardGameRepo;
        private Mock<IRepository<Club>> mockClubRepo;
        private BoardGameSessionsService service;

        [SetUp]
        public void SetUp()
        {
            mockGameSessionRepo = new Mock<IRepository<GameSession>>();
            mockClubBoardGameRepo = new Mock<IRepository<ClubBoardGame>>();
            mockBoardGameRepo = new Mock<IRepository<BoardGame>>();
            mockClubRepo = new Mock<IRepository<Club>>();

            service = new BoardGameSessionsService(
                mockGameSessionRepo.Object,
                mockClubBoardGameRepo.Object,
                mockBoardGameRepo.Object,
                mockClubRepo.Object);
        }

        [Test]
        public async Task AddGameSessionAsync_AddsNewSessionAndReturnsId()
        {
            var organizerId = Guid.NewGuid();
            var model = new AddGameSessionViewModel
            {
                StartHour = 18,
                EndHour = 22,
                MaxPlayers = 6,
                BoardGameId = Guid.NewGuid(),
                ClubId = Guid.NewGuid()
            };

            GameSession addedSession = null!;

            mockGameSessionRepo
                .Setup(r => r.AddAsync(It.IsAny<GameSession>()))
                .Callback<GameSession>(gs => addedSession = gs)
                .Returns(Task.CompletedTask);

            mockGameSessionRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var resultId = await service.AddGameSessionAsync(model, organizerId);

            Assert.IsNotNull(addedSession);
            Assert.That(addedSession.Id, Is.EqualTo(resultId));
            Assert.That(addedSession.StartTime, Is.EqualTo(DateTime.Today.AddDays(7).AddHours(model.StartHour)));
            Assert.That(addedSession.EndTime, Is.EqualTo(DateTime.Today.AddDays(7).AddHours(model.EndHour)));
            Assert.That(addedSession.MaxPlayers, Is.EqualTo(model.MaxPlayers));
            Assert.That(addedSession.CurrentPlayers, Is.EqualTo(0));
            Assert.That(addedSession.BoardGameId, Is.EqualTo(model.BoardGameId));
            Assert.That(addedSession.ClubId, Is.EqualTo(model.ClubId));
            Assert.That(addedSession.OrganizerId, Is.EqualTo(organizerId));
            Assert.IsFalse(addedSession.IsDeleted);

            mockGameSessionRepo.Verify(r => r.AddAsync(It.IsAny<GameSession>()), Times.Once);
            mockGameSessionRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void AddGameSessionAsync_ThrowsException_WhenAddAsyncFails()
        {
            var organizerId = Guid.NewGuid();
            var model = new AddGameSessionViewModel
            {
                StartHour = 18,
                EndHour = 22,
                MaxPlayers = 6,
                BoardGameId = Guid.NewGuid(),
                ClubId = Guid.NewGuid()
            };

            mockGameSessionRepo
                .Setup(r => r.AddAsync(It.IsAny<GameSession>()))
                .ThrowsAsync(new Exception("DB error"));

            Assert.ThrowsAsync<Exception>(async () => await service.AddGameSessionAsync(model, organizerId));
        }

        [Test]
        public async Task AddGameSessionAsync_StillReturnsId_WhenSaveChangesReturnsZero()
        {
            var organizerId = Guid.NewGuid();
            var model = new AddGameSessionViewModel
            {
                StartHour = 18,
                EndHour = 22,
                MaxPlayers = 6,
                BoardGameId = Guid.NewGuid(),
                ClubId = Guid.NewGuid()
            };

            GameSession addedSession = null!;

            mockGameSessionRepo
                .Setup(r => r.AddAsync(It.IsAny<GameSession>()))
                .Callback<GameSession>(gs => addedSession = gs)
                .Returns(Task.CompletedTask);

            mockGameSessionRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var resultId = await service.AddGameSessionAsync(model, organizerId);

            Assert.AreEqual(addedSession.Id, resultId);            
        }

        [Test]
        public async Task GetAllBoardGamesAsync_ReturnsOnlyNotDeletedGames()
        {
            var games = new List<BoardGame>
            {
                new BoardGame { Id = Guid.NewGuid(), Title = "Game 1", IsDeleted = false },
                new BoardGame { Id = Guid.NewGuid(), Title = "Game 2", IsDeleted = false },
            };

            var mockQueryable = games.BuildMock();

            mockBoardGameRepo.Setup(r => r.All()).Returns(mockQueryable);

            var result = await service.GetAllBoardGamesAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(g => g.Text == "Game 1" || g.Text == "Game 2"));
            Assert.That(result.All(g => Guid.TryParse(g.Value, out _)));
        }

        [Test]
        public async Task GetAllClubsAsync_ReturnsOnlyNotDeletedClubs()
        {
            var clubs = new List<Club>
            {
                new Club { Id = Guid.NewGuid(), Name = "Club 1", IsDeleted = false },
                new Club { Id = Guid.NewGuid(), Name = "Club 2", IsDeleted = false },
                new Club { Id = Guid.NewGuid(), Name = "Deleted Club", IsDeleted = true }
            };

            var mockQueryable = clubs
                .Where(c => !c.IsDeleted) 
                .ToList()                
                .BuildMock(); 

            mockClubRepo.Setup(r => r.All()).Returns(mockQueryable);

            var result = await service.GetAllClubsAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(c => c.Text == "Club 1"));
            Assert.That(result.Any(c => c.Text == "Club 2"));
            Assert.That(result.All(c => Guid.TryParse(c.Value, out _)));
        }

        [Test]
        public async Task GetManageViewModelAsync_ReturnsCorrectViewModels()
        {
            // Arrange
            var clubId = Guid.NewGuid();
            var boardGameId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddHours(2);

            var clubBoardGames = new List<ClubBoardGame>
            {
                new ClubBoardGame
                {
                    ClubId = clubId,
                    BoardGameId = boardGameId,
                    IsDeleted = false,
                    BoardGame = new BoardGame
                    {
                        Id = boardGameId,
                        Title = "Catan"
                    }
                }
            }.BuildMock();

            var gameSessions = new List<GameSession>
            {
                new GameSession
                {
                    Id = sessionId,
                    ClubId = clubId,
                    BoardGameId = boardGameId,
                    StartTime = startTime,
                    EndTime = endTime
                }
            }.BuildMock();

            mockClubBoardGameRepo
                .Setup(r => r.All())
                .Returns(clubBoardGames);

            mockGameSessionRepo
                .Setup(r => r.All())
                .Returns(gameSessions);

            var result = (await service.GetManageViewModelAsync(clubId)).ToList();

            Assert.That(result, Has.Count.EqualTo(1));

            var viewModel = result[0];
            Assert.That(viewModel.Id, Is.EqualTo(sessionId));
            Assert.That(viewModel.ClubId, Is.EqualTo(clubId));
            Assert.That(viewModel.BoardGameId, Is.EqualTo(boardGameId));
            Assert.That(viewModel.BoardGameTitle, Is.EqualTo("Catan"));
            Assert.That(viewModel.StartTime, Is.EqualTo(startTime));
            Assert.That(viewModel.EndTime, Is.EqualTo(endTime));
        }

        [Test]
        public async Task GetManageViewModelAsync_ReturnsViewModelWithEmptySession_WhenNoMatchingSession()
        {
            var clubId = Guid.NewGuid();
            var boardGameId = Guid.NewGuid();

            var clubBoardGames = new List<ClubBoardGame>
            {
                new ClubBoardGame
                {
                    ClubId = clubId,
                    BoardGameId = boardGameId,
                    IsDeleted = false,
                    BoardGame = new BoardGame
                    {
                        Id = boardGameId,
                        Title = "Ticket to Ride"
                    }
                }
            }.BuildMock();

            var emptySessions = new List<GameSession>().BuildMock();

            mockClubBoardGameRepo
                .Setup(r => r.All())
                .Returns(clubBoardGames);

            mockGameSessionRepo
                .Setup(r => r.All())
                .Returns(emptySessions);

            var result = (await service.GetManageViewModelAsync(clubId)).ToList();

            Assert.That(result, Has.Count.EqualTo(1));

            var viewModel = result[0];
            Assert.That(viewModel.Id, Is.EqualTo(Guid.Empty));
            Assert.That(viewModel.ClubId, Is.EqualTo(clubId));
            Assert.That(viewModel.BoardGameId, Is.EqualTo(boardGameId));
            Assert.That(viewModel.BoardGameTitle, Is.EqualTo("Ticket to Ride"));
            Assert.That(viewModel.StartTime, Is.Null);
            Assert.That(viewModel.EndTime, Is.Null);
        }

        [Test]
        public async Task SaveGameSessionAsync_CreatesNewSession_WhenNoneExists()
        {
            var boardGameId = Guid.NewGuid();
            var clubId = Guid.NewGuid();
            var organizerId = Guid.NewGuid();
            var startTime = new DateTime(2025, 8, 10, 18, 0, 0);
            var endTime = startTime.AddHours(2);
            var isActive = true;

            var clubBoardGame = new ClubBoardGame
            {
                BoardGameId = boardGameId,
                ClubId = clubId,
                IsDeleted = false,
                BoardGame = new BoardGame
                {
                    Id = boardGameId,
                    Title = "Catan",
                    MaxPlayers = 4
                }
            };

            var clubBoardGames = new List<ClubBoardGame> { clubBoardGame }                
                .BuildMock();

            var emptySessions = new List<GameSession>()
                .BuildMock();

            mockClubBoardGameRepo.Setup(r => r.All())
                .Returns(clubBoardGames);

            mockGameSessionRepo.Setup(r => r.All())
                .Returns(emptySessions);

            GameSession? savedSession = null;

            mockGameSessionRepo
                .Setup(r => r.AddAsync(It.IsAny<GameSession>()))
                .Callback<GameSession>(session => savedSession = session)
                .Returns(Task.CompletedTask);

            mockGameSessionRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var resultId = await service.SaveGameSessionAsync(boardGameId, startTime, endTime, organizerId, isActive);

            Assert.That(savedSession, Is.Not.Null);
            Assert.That(savedSession.BoardGameId, Is.EqualTo(boardGameId));
            Assert.That(savedSession.ClubId, Is.EqualTo(clubId));
            Assert.That(savedSession.StartTime, Is.EqualTo(startTime));
            Assert.That(savedSession.EndTime, Is.EqualTo(endTime));
            Assert.That(savedSession.OrganizerId, Is.EqualTo(organizerId));
            Assert.That(savedSession.MaxPlayers, Is.EqualTo(4));
            Assert.That(savedSession.CurrentPlayers, Is.EqualTo(0));
            Assert.That(savedSession.IsDeleted, Is.False);
            Assert.That(resultId, Is.EqualTo(savedSession.Id));
        }

        [Test]
        public async Task SaveGameSessionAsync_UpdatesExistingSession_WhenSessionExists()
        {
            var boardGameId = Guid.NewGuid();
            var clubId = Guid.NewGuid();
            var organizerId = Guid.NewGuid();
            var startTime = new DateTime(2025, 8, 10, 18, 0, 0);
            var endTime = startTime.AddHours(2);
            var isActive = false;

            var clubBoardGame = new ClubBoardGame
            {
                BoardGameId = boardGameId,
                ClubId = clubId,
                IsDeleted = false,
                BoardGame = new BoardGame
                {
                    Id = boardGameId,
                    Title = "Catan",
                    MaxPlayers = 4
                }
            };

            var clubBoardGames = new List<ClubBoardGame> { clubBoardGame }
                .BuildMock();

            var existingSession = new GameSession
            {
                Id = Guid.NewGuid(),
                BoardGameId = boardGameId,
                ClubId = clubId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                IsDeleted = false
            };

            var sessions = new List<GameSession> { existingSession }
                .BuildMock();

            mockClubBoardGameRepo.Setup(r => r.All()).Returns(clubBoardGames);
            mockGameSessionRepo.Setup(r => r.All()).Returns(sessions);

            mockGameSessionRepo.Setup(r => r.Update(It.IsAny<GameSession>()));
            mockGameSessionRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var resultId = await service.SaveGameSessionAsync(boardGameId, startTime, endTime, organizerId, isActive);

            Assert.That(resultId, Is.EqualTo(existingSession.Id));
            mockGameSessionRepo.Verify(r => r.Update(It.Is<GameSession>(s =>
                s.Id == existingSession.Id &&
                s.StartTime == startTime &&
                s.EndTime == endTime &&
                s.IsDeleted == !isActive
            )), Times.Once);
            mockGameSessionRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void SaveGameSessionAsync_ThrowsException_WhenClubBoardGameNotFound()
        {
            var boardGameId = Guid.NewGuid();
            var startTime = DateTime.Now;
            var endTime = DateTime.Now.AddHours(2);
            var organizerId = Guid.NewGuid();
            var isActive = true;

            var emptyClubBoardGames = new List<ClubBoardGame>().BuildMock();

            mockClubBoardGameRepo.Setup(r => r.All()).Returns(emptyClubBoardGames);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await service.SaveGameSessionAsync(boardGameId, startTime, endTime, organizerId, isActive)
            );
        }
    }
}
