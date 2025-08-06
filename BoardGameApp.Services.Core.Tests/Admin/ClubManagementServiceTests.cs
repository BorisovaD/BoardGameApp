namespace BoardGameApp.Services.Core.Tests.Admin
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Admin;
    using BoardGameApp.Web.ViewModels.Admin.ClubManagement;
    using Microsoft.AspNetCore.Identity;
    using NUnit.Framework;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class ClubManagementServiceTests
    {
        private Mock<IRepository<Club>> mockClubRepo;
        private Mock<IRepository<City>> mockCityRepo;
        private ClubManagementService service;

        [SetUp]
        public void SetUp()
        {
            mockClubRepo = new Mock<IRepository<Club>>();
            mockCityRepo = new Mock<IRepository<City>>();

            var mockGameSessionRepo = new Mock<IRepository<GameSession>>();
            var mockClubCustomRepo = new Mock<IClubRepository>();
            var mockManagerRepo = new Mock<IRepository<Manager>>();
            var mockUserManager = new Mock<UserManager<BoardgameUser>>(
                Mock.Of<IUserStore<BoardgameUser>>(), null, null, null, null, null, null, null, null
            );

            service = new ClubManagementService(
                mockGameSessionRepo.Object,
                mockClubRepo.Object,
                mockClubCustomRepo.Object,
                mockManagerRepo.Object,
                mockUserManager.Object,
                mockCityRepo.Object
            );
        }

        [Test]
        public void PassAlways()
        {
            Assert.Pass();
        }

        [Test]
        public async Task AddClubAsync_ShouldAddClub_WhenCityExists()
        {
            var inputModel = new ClubManagementCreateInputModel
            {
                ClubName = "Test Club",
                Address = "Test Address",
                CityName = "Test City"
            };

            var city = new City { Id = Guid.NewGuid(), Name = "Test City" };

            mockCityRepo.Setup(r => r.All())
                .Returns(new List<City> { city }.AsQueryable());

            var result = await service.AddClubAsync(inputModel);

            Assert.That(result, Is.True);
            mockClubRepo.Verify(r => r.AddAsync(It.Is<Club>(c =>
                c.Name == inputModel.ClubName &&
                c.Address == inputModel.Address &&
                c.CityId == city.Id
            )), Times.Once);

            mockClubRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task AddClubAsync_ShouldAddCityAndClub_WhenCityDoesNotExist()
        {
            var inputModel = new ClubManagementCreateInputModel
            {
                ClubName = "New Club",
                Address = "New Address",
                CityName = "New City"
            };

            mockCityRepo.Setup(r => r.All())
                .Returns(new List<City>().AsQueryable());

            City addedCity = null;
            mockCityRepo.Setup(r => r.AddAsync(It.IsAny<City>()))
                .Callback<City>(c => addedCity = c);

            mockCityRepo.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await service.AddClubAsync(inputModel);

            Assert.That(result, Is.True);
            Assert.That(addedCity, Is.Not.Null);
            Assert.That(addedCity.Name, Is.EqualTo("New City"));

            mockClubRepo.Verify(r => r.AddAsync(It.IsAny<Club>()), Times.Once);
            mockClubRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task EditClubAsync_ReturnsFalse_WhenInputModelIsNull()
        {
            var result = await service.EditClubAsync(null);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditClubAsync_ReturnsFalse_WhenClubDoesNotExist()
        {
            var input = new ClubManagementEditInputModel
            {
                Id = Guid.NewGuid(),
                ClubName = "New Name",
                Address = "New Address",
                CityName = "New City"
            };

            mockClubRepo.Setup(r => r.All())
                .Returns(new List<Club>().AsQueryable());

            var result = await service.EditClubAsync(input);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditClubAsync_UpdatesClubAndSaves_WhenCityExists()
        {
            var input = new ClubManagementEditInputModel
            {
                Id = Guid.NewGuid(),
                ClubName = "Updated Club",
                Address = "Updated Address",
                CityName = "Existing City"
            };

            var existingCity = new City { Id = Guid.NewGuid(), Name = "Existing City" };
            var existingClub = new Club
            {
                Id = input.Id,
                Name = "Old Club",
                Address = "Old Address",
                City = existingCity
            };

            mockClubRepo.Setup(r => r.All())
                .Returns(new List<Club> { existingClub }.AsQueryable());

            mockCityRepo.Setup(r => r.All())
                .Returns(new List<City> { existingCity }.AsQueryable());

            mockClubRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await service.EditClubAsync(input);

            Assert.IsTrue(result);
            Assert.That(existingClub.Name, Is.EqualTo(input.ClubName));
            Assert.That(existingClub.Address, Is.EqualTo(input.Address));
            Assert.That(existingClub.City, Is.EqualTo(existingCity));

            mockClubRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            mockCityRepo.Verify(r => r.AddAsync(It.IsAny<City>()), Times.Never);
        }

        [Test]
        public async Task EditClubAsync_AddsNewCityAndUpdatesClub_WhenCityDoesNotExist()
        {
            var input = new ClubManagementEditInputModel
            {
                Id = Guid.NewGuid(),
                ClubName = "Updated Club",
                Address = "Updated Address",
                CityName = "New City"
            };

            var existingClub = new Club
            {
                Id = input.Id,
                Name = "Old Club",
                Address = "Old Address",
                City = new City { Id = Guid.NewGuid(), Name = "Old City" }
            };

            mockClubRepo.Setup(r => r.All())
                .Returns(new List<Club> { existingClub }.AsQueryable());

            mockCityRepo.Setup(r => r.All())
                .Returns(new List<City>().AsQueryable());

            City addedCity = null;
            mockCityRepo.Setup(r => r.AddAsync(It.IsAny<City>()))
                .Callback<City>(c => addedCity = c)
                .Returns(Task.CompletedTask);

            mockClubRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await service.EditClubAsync(input);

            Assert.IsTrue(result);
            Assert.IsNotNull(addedCity);
            Assert.That(addedCity.Name, Is.EqualTo(input.CityName));
            Assert.That(existingClub.Name, Is.EqualTo(input.ClubName));
            Assert.That(existingClub.Address, Is.EqualTo(input.Address));
            Assert.That(existingClub.City, Is.EqualTo(addedCity));

            mockCityRepo.Verify(r => r.AddAsync(It.IsAny<City>()), Times.Once);
            mockClubRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetClubForEditingAsync_ReturnsNull_WhenClubNotFound()
        {
            var clubId = Guid.NewGuid();
            var clubs = new List<Club>().AsQueryable();

            mockClubRepo.Setup(r => r.All())
                .Returns(clubs);

            var result = await service.GetClubForEditingAsync(clubId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetClubForEditingAsync_ReturnsEditModel_WhenClubExists()
        {
            var clubId = Guid.NewGuid();
            var city = new City { Id = Guid.NewGuid(), Name = "Test City" };
            var club = new Club
            {
                Id = clubId,
                Name = "Test Club",
                Address = "Test Address",
                City = city
            };

            var clubs = new List<Club> { club }.AsQueryable();

            mockClubRepo.Setup(r => r.All())
                .Returns(clubs);

            var result = await service.GetClubForEditingAsync(clubId);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(clubId));
            Assert.That(result.ClubName, Is.EqualTo("Test Club"));
            Assert.That(result.Address, Is.EqualTo("Test Address"));
            Assert.That(result.CityName, Is.EqualTo("Test City"));
        }

        [Test]
        public async Task GetClubManagementInfoAsync_ReturnsAllClubsWithManagerName()
        {
            var clubId = Guid.NewGuid();
            var city = new City { Id = Guid.NewGuid(), Name = "Test City" };
            var user = new BoardgameUser { UserName = "Manager1" };
            var gameSession = new GameSession { Organizer = user };

            var club = new Club
            {
                Id = clubId,
                Name = "Test Club",
                City = city,
                IsDeleted = false,
                GameSessions = new List<GameSession> { gameSession }
            };

            var clubs = new List<Club> { club }.AsQueryable();
            var asyncClubs = new TestAsyncEnumerable<Club>(clubs);

            mockClubRepo.Setup(r => r.All()).Returns(asyncClubs);

            var result = await service.GetClubManagementInfoAsync();

            Assert.That(result.Count(), Is.EqualTo(1));
            var first = result.First();
            Assert.That(first.Id, Is.EqualTo(clubId));
            Assert.That(first.ClubName, Is.EqualTo("Test Club"));
            Assert.That(first.CityName, Is.EqualTo("Test City"));
            Assert.That(first.IsDeleted, Is.False);
            Assert.That(first.ManagerName, Is.EqualTo("Manager1"));
        }

        [Test]
        public async Task GetClubManagementInfoAsync_ReturnsEmptyList_WhenNoClubs()
        {
            var clubs = new List<Club>().AsQueryable();
            var asyncClubs = new TestAsyncEnumerable<Club>(clubs);

            mockClubRepo.Setup(r => r.All()).Returns(asyncClubs);

            var result = await service.GetClubManagementInfoAsync();

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task ToggleClubDeletionAsync_ReturnsFalse_WhenClubNotFound()
        {
            var clubId = Guid.NewGuid();
            var clubs = new List<Club>().AsEnumerable().AsQueryable();

            mockClubRepo.Setup(r => r.All()).Returns(clubs);

            var result = await service.ToggleClubDeletionAsync(clubId);

            Assert.IsFalse(result.Success);
            Assert.IsFalse(result.IsNowDeleted);
        }

        [Test]
        public async Task ToggleClubDeletionAsync_SoftDeletesClub_WhenIsDeletedIsFalse()
        {
            var clubId = Guid.NewGuid();
            var club = new Club { Id = clubId, IsDeleted = false };
            var clubs = new List<Club> { club }.AsEnumerable().AsQueryable();

            mockClubRepo.Setup(r => r.All()).Returns(clubs);

            mockClubRepo.Setup(r => r.SoftDeleteAsync(It.IsAny<Club>()))
                .Callback<Club>(c => c.IsDeleted = true)
                .Returns(Task.CompletedTask);

            mockClubRepo.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await service.ToggleClubDeletionAsync(clubId);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(club.IsDeleted);
        }

        [Test]
        public async Task ToggleClubDeletionAsync_ReturnsClub_WhenIsDeletedIsTrue()
        {
            var clubId = Guid.NewGuid();
            var club = new Club { Id = clubId, IsDeleted = true };
            var clubs = new List<Club> { club }.AsEnumerable().AsQueryable();

            mockClubRepo.Setup(r => r.All()).Returns(clubs);

            mockClubRepo.Setup(r => r.ReturnExisting(It.IsAny<Club>()))
                .Callback<Club>(c => c.IsDeleted = false)
                .Returns(Task.CompletedTask);

            mockClubRepo.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await service.ToggleClubDeletionAsync(clubId);

            Assert.IsTrue(result.Success);
            Assert.IsFalse(club.IsDeleted);
        }
    }
}
