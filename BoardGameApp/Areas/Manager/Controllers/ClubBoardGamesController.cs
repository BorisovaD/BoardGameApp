namespace BoardGameApp.Areas.Manager.Controllers
{
    using BoardGameApp.Controllers;
    using BoardGameApp.Services.Core;
    using BoardGameApp.Services.Core.Manager.Interfaces;
    using BoardGameApp.Web.ViewModels.Admin.BoardGameManagement;
    using BoardGameApp.Web.ViewModels.Manager.ClubBoardGames;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using static BoardGameApp.GCommon.ApplicationConstants;

    [Area("Manager")]
    [Authorize(Roles = RoleManager)]
    public class ClubBoardGamesController : BaseController
    {
        private readonly ICatalogService catalogService;
        private readonly IBoardGameClubService boardGameClubService;

        public ClubBoardGamesController(ICatalogService catalogService, IBoardGameClubService boardGameClubService)
        {
            this.catalogService = catalogService;
            this.boardGameClubService = boardGameClubService;
        }

        [HttpGet]
        public async Task<IActionResult> Catalog()
        {
            try
            {
                var managerId = this.GetUserId();
                var clubId = await catalogService.GetClubIdByManagerIdAsync(managerId);

                if (clubId == null)
                {
                    return NotFound();
                }

                IEnumerable<ClubBoardGamesCatalogViewModel> allBoardGames = await this.catalogService
                    .GetAllBoardGamesAsync(clubId.Value);

                return View(allBoardGames);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Toggle([FromBody] ClubGameToggleDto dto)
        {
            try
            {
                if (dto == null || dto.ClubId == Guid.Empty || dto.GameId == Guid.Empty)
                {
                    return BadRequest("Invalid data.");
                }

                await boardGameClubService.ToggleGameInClubAsync(dto.ClubId, dto.GameId);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Index), "Home");
            }
        }
    }
}
