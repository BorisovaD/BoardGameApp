namespace BoardGameApp.Services.Core.Tests.Admin
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Admin;
    using BoardGameApp.Web.ViewModels.Admin.BoardGameManagement;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class BoardGameServiceTests
    {
        private Mock<IRepository<BoardGame>> mockBoardGameRepo;
        private Mock<IRepository<BoardgameUserFavorite>> mockFavoritesRepo;
        private Mock<UserManager<BoardgameUser>> mockUserManager;
        private BoardGameService service;

        private List<BoardGame> boardGamesData;
        private List<BoardgameUserFavorite> favoritesData;

        [SetUp]
        public void SetUp()
        {
            boardGamesData = new List<BoardGame>();
            favoritesData = new List<BoardgameUserFavorite>();

            mockBoardGameRepo = new Mock<IRepository<BoardGame>>();
            mockBoardGameRepo.Setup(r => r.All()).Returns(() => boardGamesData.AsQueryable());

            mockFavoritesRepo = new Mock<IRepository<BoardgameUserFavorite>>();

            var store = new Mock<IUserStore<BoardgameUser>>();
            mockUserManager = new Mock<UserManager<BoardgameUser>>(store.Object, null, null, null, null, null, null, null, null);

            service = new BoardGameService(
                mockBoardGameRepo.Object,
                mockFavoritesRepo.Object,
                mockUserManager.Object
            );
        }

        [Test]
        public async Task AddBoardGameAsync_ReturnsFalse_WhenUserIdIsNull()
        {
            var inputModel = new AddBoardGameInputModel
            {
                SelectedCategoryIds = new List<Guid> { Guid.NewGuid() }
            };

            var result = await service.AddBoardGameAsync(null, inputModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddBoardGameAsync_ReturnsFalse_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();

            mockUserManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((BoardgameUser)null);

            var inputModel = new AddBoardGameInputModel
            {
                SelectedCategoryIds = new List<Guid> { Guid.NewGuid() }
            };

            var result = await service.AddBoardGameAsync(userId, inputModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddBoardGameAsync_ReturnsFalse_WhenNoCategoriesSelected()
        {
            var userId = Guid.NewGuid();

            mockUserManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(new BoardgameUser());

            var inputModel = new AddBoardGameInputModel
            {
                SelectedCategoryIds = new List<Guid>()
            };

            var result = await service.AddBoardGameAsync(userId, inputModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddBoardGameAsync_ReturnsTrue_WhenValidDataProvided()
        {
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            mockUserManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(new BoardgameUser());

            mockBoardGameRepo.Setup(r => r.AddAsync(It.IsAny<BoardGame>()))
                .Returns(Task.CompletedTask);

            mockBoardGameRepo.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var inputModel = new AddBoardGameInputModel
            {
                Title = "Game Title",
                Description = "Description",
                ImageUrl = "image.jpg",
                RulesUrl = "rules.pdf",
                MinPlayers = 2,
                MaxPlayers = 4,
                Duration = 60,
                SelectedCategoryIds = new List<Guid> { categoryId }
            };

            var result = await service.AddBoardGameAsync(userId, inputModel);

            Assert.IsTrue(result);
            mockBoardGameRepo.Verify(r => r.AddAsync(It.IsAny<BoardGame>()), Times.Once);
            mockBoardGameRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task AddBoardGameToUserFavoritesListAsync_ReturnsFalse_WhenUserIdIsNull()
        {
            var result = await service.AddBoardGameToUserFavoritesListAsync(null, Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddBoardGameToUserFavoritesListAsync_ReturnsFalse_WhenGameNotFound()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            mockBoardGameRepo.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync((BoardGame)null!);

            var result = await service.AddBoardGameToUserFavoritesListAsync(userId, gameId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task AddBoardGameToUserFavoritesListAsync_ReturnsTrue_WhenGameAlreadyInFavorites()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            mockBoardGameRepo.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync(new BoardGame());

            mockFavoritesRepo.Setup(r =>
                r.FirstOrDefaultAsync(It.IsAny<Expression<Func<BoardgameUserFavorite, bool>>>())
            ).ReturnsAsync(new BoardgameUserFavorite());

            var result = await service.AddBoardGameToUserFavoritesListAsync(userId, gameId);

            Assert.IsTrue(result);
            mockFavoritesRepo.Verify(r => r.ReturnExisting(It.IsAny<BoardgameUserFavorite>()), Times.Once);
            mockFavoritesRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task AddBoardGameToUserFavoritesListAsync_ReturnsTrue_WhenGameIsNewFavorite()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            mockBoardGameRepo.Setup(r => r.GetByIdAsync(gameId))
                .ReturnsAsync(new BoardGame());

            mockFavoritesRepo.Setup(r =>
                r.FirstOrDefaultAsync(It.IsAny<Expression<Func<BoardgameUserFavorite, bool>>>())
            ).ReturnsAsync((BoardgameUserFavorite)null!);

            mockFavoritesRepo.Setup(r => r.AddAsync(It.IsAny<BoardgameUserFavorite>()))
                .Returns(Task.CompletedTask);

            mockFavoritesRepo.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await service.AddBoardGameToUserFavoritesListAsync(userId, gameId);

            Assert.IsTrue(result);
            mockFavoritesRepo.Verify(r => r.AddAsync(It.IsAny<BoardgameUserFavorite>()), Times.Once);
            mockFavoritesRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllBoardGamesAsync_ReturnsOnlyNotDeletedGames()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid(); 

            boardGamesData.Clear();
            boardGamesData.Add(new BoardGame
            {
                Id = gameId,
                Title = "Saved Game", 
                IsDeleted = false,
                FavoritedByUsers = new List<BoardgameUserFavorite>
        {
            new BoardgameUserFavorite { UserId = userId, BoardGameId = gameId }
        }
            });

            boardGamesData.Add(new BoardGame
            {
                Id = Guid.NewGuid(),
                Title = "Deleted Game",
                IsDeleted = true
            });

            mockBoardGameRepo.Setup(r => r.All()).Returns(new TestAsyncQueryable<BoardGame>(boardGamesData.AsQueryable()));

            var result = await service.GetAllBoardGamesAsync(userId);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Saved Game")); 
        }

        [Test]
        public async Task GetAllBoardGamesAsync_SetsIsSavedTrue_WhenUserHasFavoritedTheGame()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid(); 

            boardGamesData.Clear(); 

            boardGamesData.Add(new BoardGame
            {
                Id = gameId,
                Title = "Saved Game",
                IsDeleted = false,
                FavoritedByUsers = new List<BoardgameUserFavorite>
        {
            new BoardgameUserFavorite { UserId = userId, BoardGameId = gameId }
        }
            });

            mockBoardGameRepo.Setup(r => r.All()).Returns(new TestAsyncQueryable<BoardGame>(boardGamesData.AsQueryable()));

            var result = await service.GetAllBoardGamesAsync(userId);

            var game = result.First();
            Assert.IsTrue(game.IsSaved);
        }

        [Test]
        public async Task GetAllBoardGamesAsync_SetsIsSavedFalse_WhenUserIdIsNull()
        {
            var gameId = Guid.NewGuid();

            boardGamesData.Clear();
            boardGamesData.Add(new BoardGame
            {
                Id = gameId,
                Title = "Unsaved Game",
                IsDeleted = false,
                FavoritedByUsers = new List<BoardgameUserFavorite>()
            });

            mockBoardGameRepo.Setup(r => r.All()).Returns(new TestAsyncQueryable<BoardGame>(boardGamesData));

            var result = await service.GetAllBoardGamesAsync(null);

            var game = result.First();
            Assert.IsFalse(game.IsSaved);
        }

        [Test]
        public async Task GetBoardGameDetailsAsync_ReturnsDetailsViewModel_WithCorrectDataAndIsSaved()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            var boardGame = new BoardGame
            {
                Id = gameId,
                Title = "Test Game",
                Description = "Test Description",
                ImageUrl = "test-image-url",
                RulesUrl = "test-rules-url",
                MinPlayers = 2,
                MaxPlayers = 5,
                Duration = 60,
                IsDeleted = false,
                BoardGameCategories = new List<BoardGameCategory>
                {
                    new BoardGameCategory
                    {
                        Category = new Category { Name = "Strategy" }
                    },
                    new BoardGameCategory
                    {
                        Category = new Category { Name = "Family" }
                    }
                },
                FavoritedByUsers = new List<BoardgameUserFavorite>
                {
                    new BoardgameUserFavorite { UserId = userId, BoardGameId = gameId }
                }
            };

            var data = new List<BoardGame> { boardGame }.AsQueryable();

            mockBoardGameRepo.Setup(r => r.All())
                .Returns(data);

            var result = await service.GetBoardGameDetailsAsync(gameId, userId);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(gameId.ToString()));
            Assert.That(result.Title, Is.EqualTo("Test Game"));
            Assert.That(result.IsSaved, Is.EqualTo(true));
            Assert.That(result.Categories, Has.Member("Strategy").And.Member("Family"));
        }

        [Test]
        public async Task GetBoardGameDetailsAsync_ReturnsNull_WhenIdIsNull()
        {
            var result = await service.GetBoardGameDetailsAsync(null, Guid.NewGuid());
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetBoardGameDetailsAsync_ReturnsNull_WhenGameNotFound()
        {
            var unknownId = Guid.NewGuid();

            var data = new List<BoardGame>().AsQueryable();

            mockBoardGameRepo.Setup(r => r.All())
                .Returns(data);

            var result = await service.GetBoardGameDetailsAsync(unknownId, Guid.NewGuid());

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetBoardGameForDeletingAsync_ReturnsCorrectModel_WhenGameExists()
        {
            var gameId = Guid.NewGuid();

            boardGamesData.Clear();
            boardGamesData.Add(new BoardGame
            {
                Id = gameId,
                Title = "Test Game",
                ImageUrl = "image.jpg",
                IsDeleted = false
            });

            var result = await service.GetBoardGameForDeletingAsync(null, gameId);

            Assert.IsNotNull(result);
            Assert.That(result!.Id, Is.EqualTo(gameId));
            Assert.That(result.Title, Is.EqualTo("Test Game"));
            Assert.That(result.ImageUrl, Is.EqualTo("image.jpg"));
        }

        [Test]
        public async Task GetBoardGameForDeletingAsync_ReturnsNull_WhenGameNotFound()
        {
            boardGamesData.Clear(); 

            var result = await service.GetBoardGameForDeletingAsync(null, Guid.NewGuid());

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetBoardGameForEditingAsync_ReturnsCorrectModel_WhenGameExists()
        {
            var gameId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            boardGamesData.Clear();
            boardGamesData.Add(new BoardGame
            {
                Id = gameId,
                Title = "Game Title",
                Description = "Description",
                ImageUrl = "img.jpg",
                RulesUrl = "rules.pdf",
                MinPlayers = 2,
                MaxPlayers = 6,
                Duration = 60,
                IsDeleted = false,
                BoardGameCategories = new List<BoardGameCategory>
                {
                    new BoardGameCategory
                    {
                        CategoryId = categoryId,
                        Category = new Category { Id = categoryId, Name = "Strategy" }
                    }
                }
            });

            var result = await service.GetBoardGameForEditingAsync(null, gameId);

            Assert.IsNotNull(result);
            Assert.That(result!.Id, Is.EqualTo(gameId));
            Assert.That(result.Title, Is.EqualTo("Game Title"));
            Assert.That(result.Description, Is.EqualTo("Description"));
            Assert.That(result.ImageUrl, Is.EqualTo("img.jpg"));
            Assert.That(result.RulesUrl, Is.EqualTo("rules.pdf"));
            Assert.That(result.MinPlayers, Is.EqualTo(2));
            Assert.That(result.MaxPlayers, Is.EqualTo(6));
            Assert.That(result.Duration, Is.EqualTo(60));
            Assert.Contains(categoryId, result.SelectedCategoryIds);
        }

        [Test]
        public async Task GetBoardGameForEditingAsync_ReturnsNull_WhenGameNotFound()
        {
            boardGamesData.Clear();

            var result = await service.GetBoardGameForEditingAsync(null, Guid.NewGuid());

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetUserFavoritesBoardGameAsync_ReturnsFavorites_WhenUserHasFavorites()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            boardGamesData.Clear();
            favoritesData.Clear();

            var category = new Category
            {
                Id = categoryId,
                Name = "Strategy"
            };

            var boardGameCategory = new BoardGameCategory
            {
                BoardGameId = gameId,
                CategoryId = categoryId,
                Category = category
            };

            var boardGame = new BoardGame
            {
                Id = gameId,
                Title = "Catan",
                Description = "Trade and build",
                ImageUrl = "catan.jpg",
                RulesUrl = "rules.pdf",
                MinPlayers = 3,
                MaxPlayers = 4,
                Duration = 90,
                IsDeleted = false,
                BoardGameCategories = new List<BoardGameCategory> { boardGameCategory }
            };

            boardGameCategory.BoardGame = boardGame;

            boardGamesData.Add(boardGame);

            favoritesData.Add(new BoardgameUserFavorite
            {
                UserId = userId,
                BoardGameId = gameId,
                BoardGame = boardGame
            });

            mockFavoritesRepo
                .Setup(r => r.All())
                .Returns(favoritesData.AsQueryable());

            var result = await service.GetUserFavoritesBoardGameAsync(userId);

            Assert.IsNotNull(result);
            Assert.That(result, Is.Not.Empty);

            var favorite = result!.First();

            Assert.That(favorite.Title, Is.EqualTo("Catan"));
            Assert.That(favorite.Categories, Is.Not.Empty);
            Assert.That(favorite.Categories.First(), Is.EqualTo("Strategy"));
        }

        [Test]
        public async Task GetUserFavoritesBoardGameAsync_ReturnsEmpty_WhenUserHasNoFavorites()
        {
            var userId = Guid.NewGuid();
            favoritesData.Clear();

            var result = await service.GetUserFavoritesBoardGameAsync(userId);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result!);
        }

        [Test]
        public async Task GetUserFavoritesBoardGameAsync_ReturnsEmpty_WhenUserIdIsNull()
        {
            var result = await service.GetUserFavoritesBoardGameAsync(null);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result!);
        }

        [Test]
        public async Task PersistUpdatedGameBoardAsync_ReturnsTrue_WhenUpdateIsSuccessful()
        {
            var userId = Guid.NewGuid();
            var gameId = Guid.NewGuid();

            var inputModel = new EditBoardGameInputModel
            {
                Id = gameId,
                Title = "New Title",
                Description = "New Description",
                ImageUrl = "newimage.jpg",
                RulesUrl = "newrules.pdf",
                MinPlayers = 2,
                MaxPlayers = 6,
                Duration = 120,
                SelectedCategoryIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };

            var boardGame = new BoardGame
            {
                Id = gameId,
                Title = "Old Title",
                Description = "Old Description",
                ImageUrl = "oldimage.jpg",
                RulesUrl = "oldrules.pdf",
                MinPlayers = 1,
                MaxPlayers = 4,
                Duration = 90,
                BoardGameCategories = new List<BoardGameCategory>
                {
                    new BoardGameCategory { BoardGameId = gameId, CategoryId = Guid.NewGuid() }
                }
            };

            mockUserManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(new BoardgameUser { Id = userId });

            mockBoardGameRepo.Setup(r => r.All())
                .Returns(new List<BoardGame> { boardGame }.AsQueryable());

            mockBoardGameRepo.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await service.PersistUpdatedGameBoardAsync(userId, inputModel);

            Assert.IsTrue(result);
            Assert.That(boardGame.Title, Is.EqualTo("New Title"));
            Assert.That(boardGame.Description, Is.EqualTo("New Description"));
            Assert.That(boardGame.ImageUrl, Is.EqualTo("newimage.jpg"));
            Assert.That(boardGame.RulesUrl, Is.EqualTo("newrules.pdf"));
            Assert.That(boardGame.MinPlayers, Is.EqualTo(2));
            Assert.That(boardGame.MaxPlayers, Is.EqualTo(6));
            Assert.That(boardGame.Duration, Is.EqualTo(120));
            Assert.That(boardGame.BoardGameCategories.Count, Is.EqualTo(inputModel.SelectedCategoryIds.Count));
        }

        [Test]
        public async Task PersistUpdatedGameBoardAsync_ReturnsFalse_WhenUserOrBoardGameIsNull()
        {
            var userId = Guid.NewGuid();
            var inputModel = new EditBoardGameInputModel { Id = Guid.NewGuid() };

            mockUserManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((BoardgameUser)null);

            var result1 = await service.PersistUpdatedGameBoardAsync(userId, inputModel);

            mockUserManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(new BoardgameUser { Id = userId });

            mockBoardGameRepo.Setup(r => r.All())
                .Returns(new List<BoardGame>().AsQueryable());

            var result2 = await service.PersistUpdatedGameBoardAsync(userId, inputModel);

            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }

        [Test]
        public async Task RemoveBoardGameFromUserFavoritesListAsync_RemovesFavorite_WhenExists()
        {
            var userId = Guid.NewGuid();
            var boardGameId = Guid.NewGuid();

            var favorite = new BoardgameUserFavorite
            {
                UserId = userId,
                BoardGameId = boardGameId
            };

            var favoritesRepoMock = new Mock<IRepository<BoardgameUserFavorite>>();
            favoritesRepoMock.Setup(r =>
                r.FirstOrDefaultAsync(It.IsAny<Expression<Func<BoardgameUserFavorite, bool>>>()))
                .ReturnsAsync(favorite);

            favoritesRepoMock.Setup(r => r.Delete(favorite));
            favoritesRepoMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = new BoardGameService(
                mockBoardGameRepo.Object,
                favoritesRepoMock.Object,
                mockUserManager.Object
            );

            var result = await service.RemoveBoardGameFromUserFavoritesListAsync(userId, boardGameId);

            Assert.IsTrue(result);
            favoritesRepoMock.Verify(r => r.Delete(favorite), Times.Once);
            favoritesRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task RemoveBoardGameFromUserFavoritesListAsync_ReturnsFalse_WhenUserIdIsNull()
        {
            var boardGameId = Guid.NewGuid();

            var service = new BoardGameService(
                mockBoardGameRepo.Object,
                Mock.Of<IRepository<BoardgameUserFavorite>>(),
                mockUserManager.Object
            );

            var result = await service.RemoveBoardGameFromUserFavoritesListAsync(null, boardGameId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task RemoveBoardGameFromUserFavoritesListAsync_ReturnsFalse_WhenFavoriteNotFound()
        {
            var userId = Guid.NewGuid();
            var boardGameId = Guid.NewGuid();

            var favoritesRepoMock = new Mock<IRepository<BoardgameUserFavorite>>();
            favoritesRepoMock.Setup(r =>
                r.FirstOrDefaultAsync(It.IsAny<Expression<Func<BoardgameUserFavorite, bool>>>()))
                .ReturnsAsync((BoardgameUserFavorite?)null);

            var service = new BoardGameService(
                mockBoardGameRepo.Object,
                favoritesRepoMock.Object,
                mockUserManager.Object
    );

            var result = await service.RemoveBoardGameFromUserFavoritesListAsync(userId, boardGameId);

            Assert.IsFalse(result);
            favoritesRepoMock.Verify(r => r.Delete(It.IsAny<BoardgameUserFavorite>()), Times.Never);
            favoritesRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task SoftDeleteBoardGameAsync_SetsIsDeletedTrue_WhenUserAndGameExist()
        {
            var userId = Guid.NewGuid();
            var boardGameId = Guid.NewGuid();

            var inputModel = new DeleteBoardGameInputModel
            {
                Id = boardGameId
            };

            var user = new BoardgameUser { Id = userId };
            var boardGame = new BoardGame { Id = boardGameId, IsDeleted = false };

            mockUserManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            mockBoardGameRepo.Setup(r => r.GetByIdAsync(boardGameId))
                .ReturnsAsync(boardGame);

            mockBoardGameRepo.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await service.SoftDeleteBoardGameAsync(userId, inputModel);

            Assert.IsTrue(result);
            Assert.IsTrue(boardGame.IsDeleted);
            mockBoardGameRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task SoftDeleteBoardGameAsync_ReturnsFalse_WhenUserIdIsNull()
        {
            var inputModel = new DeleteBoardGameInputModel
            {
                Id = Guid.NewGuid()
            };

            var result = await service.SoftDeleteBoardGameAsync(null, inputModel);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task SoftDeleteBoardGameAsync_ReturnsFalse_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            var inputModel = new DeleteBoardGameInputModel
            {
                Id = Guid.NewGuid()
            };

            mockUserManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((BoardgameUser?)null);

            var result = await service.SoftDeleteBoardGameAsync(userId, inputModel);

            Assert.IsFalse(result);
            mockBoardGameRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task SoftDeleteBoardGameAsync_ReturnsFalse_WhenBoardGameNotFound()
        {
            var userId = Guid.NewGuid();
            var boardGameId = Guid.NewGuid();

            var inputModel = new DeleteBoardGameInputModel { Id = boardGameId };

            var user = new BoardgameUser { Id = userId };

            mockUserManager.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            mockBoardGameRepo.Setup(r => r.GetByIdAsync(boardGameId))
                .ReturnsAsync((BoardGame?)null);

            var result = await service.SoftDeleteBoardGameAsync(userId, inputModel);

            Assert.IsFalse(result);
            mockBoardGameRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
