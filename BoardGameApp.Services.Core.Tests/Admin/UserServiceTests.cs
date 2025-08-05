namespace BoardGameApp.Services.Core.Tests.Admin
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Admin;
    using BoardGameApp.Web.ViewModels.Admin.UserManagement;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using MockQueryable.Moq;
    using Moq.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class UserServiceTests
    {
        private Mock<UserManager<BoardgameUser>> userManagerMock;
        private Mock<RoleManager<IdentityRole<Guid>>> roleManagerMock;
        private Mock<IRepository<Manager>> managerRepositoryMock;
        private UserService userService;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<BoardgameUser>>();
            userManagerMock = new Mock<UserManager<BoardgameUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole<Guid>>>();
            roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(roleStoreMock.Object, null, null, null, null);

            managerRepositoryMock = new Mock<IRepository<Manager>>();

            userService = new UserService(userManagerMock.Object, roleManagerMock.Object, managerRepositoryMock.Object);
        }

        [Test]
        public void PassAlways()
        {
            Assert.Pass();
        }

        [Test]
        public async Task AssignRoleAsync_ShouldReturnTrue_WhenRoleAssignedSuccessfully()
        {            
            var userId = Guid.NewGuid();
            var roleName = "Admin";

            var inputModel = new RoleSelectionInputModel
            {
                UserId = userId,
                Role = roleName
            };

            var user = new BoardgameUser { Id = userId };

            userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            roleManagerMock.Setup(r => r.RoleExistsAsync(roleName))
                .ReturnsAsync(true);

            userManagerMock.Setup(u => u.AddToRoleAsync(user, roleName))
                .ReturnsAsync(IdentityResult.Success);
                        
            var result = await userService.AssignRoleAsync(inputModel);
                        
            Assert.IsTrue(result);
            userManagerMock.Verify(u => u.AddToRoleAsync(user, roleName), Times.Once);
        }

        [Test]
        public void AssignRoleAsync_ShouldThrowArgumentException_WhenUserNotFound()
        {
            var inputModel = new RoleSelectionInputModel
            {
                UserId = Guid.NewGuid(),
                Role = "Admin"
            };

            userManagerMock.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((BoardgameUser?)null);

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await userService.AssignRoleAsync(inputModel));

            Assert.That(ex.Message, Is.EqualTo("User does not exist!"));
        }

        [Test]
        public void AssignRoleAsync_ShouldThrowArgumentException_WhenRoleDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var inputModel = new RoleSelectionInputModel
            {
                UserId = userId,
                Role = "InvalidRole"
            };

            var user = new BoardgameUser { Id = userId };

            userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            roleManagerMock.Setup(r => r.RoleExistsAsync(inputModel.Role))
                .ReturnsAsync(false);

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await userService.AssignRoleAsync(inputModel));

            Assert.That(ex.Message, Is.EqualTo("Selected role is not a valid role!"));
        }

        [Test]
        public void AssignRoleAsync_ShouldThrowArgumentException_WhenAddToRoleThrows()
        {
            var userId = Guid.NewGuid();
            var roleName = "Admin";

            var inputModel = new RoleSelectionInputModel
            {
                UserId = userId,
                Role = roleName
            };

            var user = new BoardgameUser { Id = userId };

            userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            roleManagerMock.Setup(r => r.RoleExistsAsync(roleName))
                .ReturnsAsync(true);

            userManagerMock.Setup(u => u.AddToRoleAsync(user, roleName))
                .ThrowsAsync(new Exception("Some error"));

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await userService.AssignRoleAsync(inputModel));

            Assert.That(ex.Message, Does.StartWith("Unexpected error occurred while adding the user to role!"));
            Assert.IsNotNull(ex.InnerException);
            Assert.That(ex.InnerException!.Message, Is.EqualTo("Some error"));
        }

        [Test]
        public async Task DeleteUserByIdAsync_ShouldReturnTrue_WhenUserDeletedSuccessfully()
        {
            var userId = Guid.NewGuid();
            var user = new BoardgameUser { Id = userId };

            userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            userManagerMock.Setup(u => u.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var result = await userService.DeleteUserByIdAsync(userId);

            Assert.IsTrue(result);
            userManagerMock.Verify(u => u.DeleteAsync(user), Times.Once);
        }

        [Test]
        public void DeleteUserByIdAsync_ShouldThrowArgumentException_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();

            userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((BoardgameUser?)null);

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await userService.DeleteUserByIdAsync(userId));

            Assert.That(ex.Message, Is.EqualTo("User not found!"));
        }

        [Test]
        public void DeleteUserByIdAsync_ShouldThrowInvalidOperationException_WhenDeleteFails()
        {
            var userId = Guid.NewGuid();
            var user = new BoardgameUser { Id = userId };

            userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            userManagerMock.Setup(u => u.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Failed());

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await userService.DeleteUserByIdAsync(userId));

            Assert.That(ex.Message, Is.EqualTo("Failed to delete user."));
        }

        [Test]
        public async Task GetUserManagementInfoAsync_ShouldReturnAllUsersExceptGivenUserWithRoles()
        {
            var currentUserId = Guid.NewGuid();

            var users = new List<BoardgameUser>
    {
        new BoardgameUser { Id = currentUserId, Email = "current@user.com" },
        new BoardgameUser { Id = Guid.NewGuid(), Email = "user1@example.com" },
        new BoardgameUser { Id = Guid.NewGuid(), Email = null },
    };

            userManagerMock.Setup(u => u.Users).Returns(users.BuildMockDbSet().Object);

            foreach (var user in users.Where(u => u.Id != currentUserId))
            {
                userManagerMock.Setup(u => u.GetRolesAsync(user))
                    .ReturnsAsync(new List<string> { "Role1", "Role2" });
            }

            var result = await userService.GetUserManagementInfoAsync(currentUserId);

            Assert.That(result.Count(), Is.EqualTo(2));

            foreach (var userViewModel in result)
            {
                Assert.That(userViewModel.Roles, Does.Contain("Role1"));
                Assert.That(userViewModel.Roles, Does.Contain("Role2"));
                Assert.IsNotNull(userViewModel.Email);

                if (userViewModel.Id == currentUserId)
                {
                    Assert.Fail("Current user should not be included");
                }
            }

            var userWithNullEmail = result.FirstOrDefault(u => u.Email == "");
            Assert.IsNotNull(userWithNullEmail);
        }

        [Test]
        public async Task RemoveRoleAsync_ShouldRemoveRoleSuccessfully()
        {
            var userId = Guid.NewGuid();
            var roleName = "Admin";

            var user = new BoardgameUser { Id = userId, Email = "test@example.com" };

            var inputModel = new RoleSelectionInputModel
            {
                UserId = userId,
                Role = roleName
            };

            userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            roleManagerMock.Setup(r => r.RoleExistsAsync(roleName))
                .ReturnsAsync(true);

            userManagerMock.Setup(u => u.RemoveFromRoleAsync(user, roleName))
                .ReturnsAsync(IdentityResult.Success);

            var result = await userService.RemoveRoleAsync(inputModel);

            Assert.IsTrue(result);
            userManagerMock.Verify(u => u.RemoveFromRoleAsync(user, roleName), Times.Once);
        }

        [Test]
        public void RemoveRoleAsync_ShouldThrow_WhenUserNotFound()
        {
            var inputModel = new RoleSelectionInputModel
            {
                UserId = Guid.NewGuid(),
                Role = "Admin"
            };

            userManagerMock.Setup(u => u.FindByIdAsync(inputModel.UserId.ToString()))
                .ReturnsAsync((BoardgameUser?)null);

            var ex = Assert.ThrowsAsync<ArgumentException>(
                async () => await userService.RemoveRoleAsync(inputModel));

            Assert.That(ex!.Message, Is.EqualTo("User does not exist!"));
        }

        [Test]
        public void RemoveRoleAsync_ShouldThrow_WhenRoleDoesNotExist()
        {
            var user = new BoardgameUser { Id = Guid.NewGuid() };
            var inputModel = new RoleSelectionInputModel
            {
                UserId = user.Id,
                Role = "InvalidRole"
            };

            userManagerMock.Setup(u => u.FindByIdAsync(inputModel.UserId.ToString()))
                .ReturnsAsync(user);

            roleManagerMock.Setup(r => r.RoleExistsAsync(inputModel.Role))
                .ReturnsAsync(false);

            var ex = Assert.ThrowsAsync<ArgumentException>(
                async () => await userService.RemoveRoleAsync(inputModel));

            Assert.That(ex!.Message, Is.EqualTo("Selected role is not a valid role!"));
        }

        [Test]
        public void RemoveRoleAsync_ShouldThrow_WhenUnexpectedErrorOccurs()
        {
            var user = new BoardgameUser { Id = Guid.NewGuid() };
            var inputModel = new RoleSelectionInputModel
            {
                UserId = user.Id,
                Role = "Admin"
            };

            userManagerMock.Setup(u => u.FindByIdAsync(inputModel.UserId.ToString()))
                .ReturnsAsync(user);

            roleManagerMock.Setup(r => r.RoleExistsAsync(inputModel.Role))
                .ReturnsAsync(true);

            userManagerMock.Setup(u => u.RemoveFromRoleAsync(user, inputModel.Role))
                .ThrowsAsync(new Exception("Something went wrong"));

            var ex = Assert.ThrowsAsync<ArgumentException>(
                async () => await userService.RemoveRoleAsync(inputModel));

            Assert.That(ex!.Message, Does.Contain("Unexpected error occurred while removing the user from role"));
        }
    }
}
