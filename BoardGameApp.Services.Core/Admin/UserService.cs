namespace BoardGameApp.Services.Core.Admin
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Admin.Interfaces;
    using BoardGameApp.Web.ViewModels.Admin.UserManagement;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly UserManager<BoardgameUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly IRepository<Manager> managerRepository;

        public UserService(UserManager<BoardgameUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IRepository<Manager> managerRepository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.managerRepository = managerRepository;
        }

        public async Task<bool> AssignRoleAsync(RoleSelectionInputModel inputModel)
        {
            BoardgameUser? user = await this.userManager
                .FindByIdAsync(inputModel.UserId.ToString());

            if (user == null)
            {
                throw new ArgumentException("User does not exist!");
            }

            bool roleExists = await this.roleManager.RoleExistsAsync(inputModel.Role);
            if (!roleExists)
            {
                throw new ArgumentException("Selected role is not a valid role!");
            }

            try
            {
                await this.userManager.AddToRoleAsync(user, inputModel.Role);
                return true;
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    "Unexpected error occurred while adding the user to role! Please try again later!",
                    innerException: e);
            }
        }

        public async Task<bool> DeleteUserByIdAsync(Guid userId)
        {
            BoardgameUser? user = await this.userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new ArgumentException("User not found!");
            }

            var result = await this.userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to delete user.");
            }

            return true;
        }

        public async Task<IEnumerable<UserManagementIndexViewModel>> GetUserManagementInfoAsync(Guid userId)
        {
            List<BoardgameUser> users = await userManager.Users
                .Where(u => u.Id != userId)
                .ToListAsync();

            List<UserManagementIndexViewModel> userViewModels = new List<UserManagementIndexViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);

                userViewModels.Add(new UserManagementIndexViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    Roles = roles
                });
            }

            return userViewModels;
        }

        public async Task<bool> RemoveRoleAsync(RoleSelectionInputModel inputModel)
        {
            BoardgameUser? user = await this.userManager
                .FindByIdAsync(inputModel.UserId.ToString());

            if (user == null)
            {
                throw new ArgumentException("User does not exist!");
            }

            bool roleExists = await this.roleManager.RoleExistsAsync(inputModel.Role);
            if (!roleExists)
            {
                throw new ArgumentException("Selected role is not a valid role!");
            }

            try
            {                
                await this.userManager.RemoveFromRoleAsync(user, inputModel.Role);
                return true;
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    "Unexpected error occurred while removing the user from role! Please try again later!",
                    innerException: e);
            }
        }
    }
}
