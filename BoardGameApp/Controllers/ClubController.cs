namespace BoardGameApp.Controllers
{
    using BoardGameApp.Services.Core;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.Club;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ClubController : BaseController
    {
        private readonly IClubService clubService;

        public ClubController(IClubService clubService)
        {
            this.clubService = clubService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<ClubMapViewModel> activeClubs = await clubService.GetAllActiveClubs();
                return View(activeClubs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }            
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                ClubDetailsViewModel model = await clubService.GetClubDetailsAsync(id);

                if (model == null)
                {
                    return NotFound();
                }

                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }            
        }
    }
}
