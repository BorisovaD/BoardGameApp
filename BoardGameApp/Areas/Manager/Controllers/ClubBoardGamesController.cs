namespace BoardGameApp.Areas.Manager.Controllers
{
    using BoardGameApp.Controllers;
    using BoardGameApp.Services.Core;
    using BoardGameApp.Services.Core.Manager.Interfaces;
    using BoardGameApp.Web.ViewModels.Admin.BoardGameManagement;
    using BoardGameApp.Web.ViewModels.Manager.ClubBoardGames;
    using Microsoft.AspNetCore.Mvc;

    [Area("Manager")]
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

        [HttpPost]
        public async Task<IActionResult> Toggle([FromBody] ClubGameToggleDto dto)
        {
            if (dto == null || dto.ClubId == Guid.Empty || dto.GameId == Guid.Empty)
            {
                return BadRequest("Invalid data.");
            }

            await boardGameClubService.ToggleGameInClubAsync(dto.ClubId, dto.GameId);
            return Ok();            
        }
    }
}
