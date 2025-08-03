namespace BoardGameApp.Areas.Admin.Controllers
{
    using BoardGameApp.Services.Core.Admin.Interfaces;
    using BoardGameApp.Web.ViewModels.Admin.ClubManagement;
    using BoardGameApp.Web.ViewModels.BoardGame;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using static BoardGameApp.GCommon.ApplicationConstants;

    [Authorize(Roles = RoleAdmin)]
    public class ClubManagementController : BaseAdminController
    {
        private readonly IClubManagementService clubManagementService;
        private readonly IUserService userService;

        public ClubManagementController(IClubManagementService clubManagementService, IUserService userService)
        {
            this.clubManagementService = clubManagementService;
            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            try
            {
                IEnumerable<ClubManagementIndexViewModel> allClubs = await this.clubManagementService
                .GetClubManagementInfoAsync();

                return View(allClubs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Manage));
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                var viewModel = new ClubManagementCreateInputModel();

                return View(viewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Manage));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClubManagementCreateInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            try
            {
                bool success = await this.clubManagementService
                    .AddClubAsync(inputModel);

                if (!success)
                {
                    TempData[ErrorMessageKey] = "Error occurred while adding the club!";
                }
                else
                {
                    TempData[SuccessMessageKey] = "Club created successfully!";
                }

                return this.RedirectToAction(nameof(Manage));
            }
            catch (Exception e)
            {
                TempData[ErrorMessageKey] =
                    "Unexpected error occurred while adding the club! Please contact developer team!";

                return this.RedirectToAction(nameof(Manage));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                ClubManagementEditInputModel? editFormModel = await this.clubManagementService
                .GetClubForEditingAsync(id);

                if (editFormModel == null)
                {
                    TempData[ErrorMessageKey] = "Selected Club does not exist!";

                    return this.RedirectToAction(nameof(Manage));
                }

                return this.View(editFormModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Manage));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ClubManagementEditInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            try
            {
                bool success = await this.clubManagementService
                    .EditClubAsync(inputModel);

                if (!success)
                {
                    TempData[ErrorMessageKey] = "Error occurred while updating the club!";
                }
                else
                {
                    TempData[SuccessMessageKey] = "Club updated successfully!";
                }

                return this.RedirectToAction(nameof(Manage));
            }
            catch (Exception e)
            {
                TempData[ErrorMessageKey] =
                    "Unexpected error occurred while editing the club! Please contact developer team!";

                return this.RedirectToAction(nameof(Manage));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleDelete(Guid id)
        {
            try
            {
                var (success, isNowDeleted) = await this.clubManagementService.ToggleClubDeletionAsync(id);

                if (!success)
                {
                    TempData["ErrorMessage"] = "Club not found!";
                }
                else if (isNowDeleted)
                {
                    TempData["WarningMessage"] = "Club was successfully deleted.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Club was restored successfully.";
                }

                return RedirectToAction(nameof(Manage));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Manage));
            }
        }
    }
}
