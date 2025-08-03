namespace BoardGameApp.Areas.Admin.Controllers
{
    using BoardGameApp.Services.Core.Admin.Interfaces;
    using BoardGameApp.Web.ViewModels.Admin.UserManagement;
    using BoardGameApp.Web.ViewModels.BoardGame;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using static BoardGameApp.GCommon.ApplicationConstants;

    [Authorize(Roles = RoleAdmin)]
    public class UserManagementController : BaseAdminController
    {
        private readonly IUserService userService;

        public UserManagementController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                Guid? userId = this.GetUserId();

                if (userId == null)
                {
                    return NotFound();
                }

                IEnumerable<UserManagementIndexViewModel> allUsers =
                    await this.userService.GetUserManagementInfoAsync(userId.Value);

                return View(allUsers);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(RoleSelectionInputModel inputModel)
        {
            try
            {
                await this.userService
                    .AssignRoleAsync(inputModel);
                TempData[SuccessMessageKey] = "User assigned to role successfully!";

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData[ErrorMessageKey] = e.Message;

                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(RoleSelectionInputModel inputModel)
        {
            try
            {
                await this.userService
                    .RemoveRoleAsync(inputModel);
                TempData[SuccessMessageKey] = "User role is successfully removed!";

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData[ErrorMessageKey] = e.Message;

                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                await this.userService.DeleteUserByIdAsync(userId);
                TempData[SuccessMessageKey] = "User deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData[ErrorMessageKey] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
