namespace BoardGameApp.Areas.Manager.Controllers
{
    using BoardGameApp.Controllers;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Services.Core.Manager;
    using BoardGameApp.Services.Core.Manager.Interfaces;
    using BoardGameApp.Web.ViewModels.Manager.GameSessions;
    using Microsoft.AspNetCore.Mvc;

    [Area("Manager")]
    public class GameSessionsController : BaseController
    {
        private readonly IBoardGameSessionsService boardGameSessionsService;
        private readonly ICatalogService catalogService;
        public GameSessionsController(IBoardGameSessionsService boardGameSessionsService, ICatalogService catalogService)
        {
            this.boardGameSessionsService = boardGameSessionsService;
            this.catalogService = catalogService;
        }
        public async Task<IActionResult> Manage()
        {
            var managerId = this.GetUserId();
            var clubId = await catalogService.GetClubIdByManagerIdAsync(managerId);

            if (clubId == null)
            {
                return NotFound();
            }

            IEnumerable<ManageGameSessionViewModel> model = await boardGameSessionsService.GetManageViewModelAsync(clubId.Value);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveGameSessionTime(Guid gameId, int startHour, int endHour)
        {
            var success = await boardGameSessionsService.SaveGameSessionTimeAsync(gameId, startHour, endHour);

            if (!success)
            {
                return BadRequest("Could not be saved.");
            }

            return Ok("Saved successfully!");
        }
    }
}
