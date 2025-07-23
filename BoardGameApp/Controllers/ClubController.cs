namespace BoardGameApp.Controllers
{
    using BoardGameApp.Services.Core;
    using BoardGameApp.Services.Core.Contracts;
    using Microsoft.AspNetCore.Mvc;

    public class ClubController : BaseController
    {
        private readonly IClubService clubService;

        public ClubController(IClubService clubService)
        {
            this.clubService = clubService;
        }

        public async Task<IActionResult> Index()
        {
            var activeClubs = await clubService.GetAllActiveClubs();
            return View(activeClubs);
        }
    }
}
