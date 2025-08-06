namespace BoardGameApp.Areas.Manager.Controllers
{
    using BoardGameApp.Controllers;
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Services.Core.Manager;
    using BoardGameApp.Services.Core.Manager.Interfaces;
    using BoardGameApp.Web.ViewModels.Manager.GameSessions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using static BoardGameApp.GCommon.ApplicationConstants;

    [Area("Manager")]
    [Authorize(Roles = RoleManager)]
    public class GameSessionsController : BaseController
    {
        private readonly IBoardGameSessionsService boardGameSessionsService;
        private readonly ICatalogService catalogService;
        
        public GameSessionsController(IBoardGameSessionsService boardGameSessionsService, ICatalogService catalogService)
        {
            this.boardGameSessionsService = boardGameSessionsService;
            this.catalogService = catalogService;            
        }
        
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Manage));
            }
        }
              
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(GameSessionInputModel model)
        {
            var startDateTime = DateTime.Today.AddHours(model.StartTime);
            var endDateTime = DateTime.Today.AddHours(model.EndTime);

            try
            {
                var organizerId = this.GetUserId();

                if (!organizerId.HasValue)
                {
                    Console.WriteLine("Organizer ID not found. User may not be logged in.");
                    return Unauthorized(); 
                }

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("Model state is invalid.");
                    return BadRequest("Invalid input.");
                }

                Console.WriteLine($"Saving session for user {organizerId.Value}, GameId: {model.BoardGameId}");

                Guid sessionId = await this.boardGameSessionsService.SaveGameSessionAsync(
                    model.BoardGameId,
                    startDateTime,
                    endDateTime,
                    organizerId.Value,
                    model.IsActive);
                                
                return RedirectToAction("Index", "GameSession", new { id = sessionId, area = "" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while saving session: " + ex.Message);
                return StatusCode(500, "An error occurred while saving the game session.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {            
            try
            {
                var model = new AddGameSessionViewModel
                {
                    AvailableBoardGames = await boardGameSessionsService.GetAllBoardGamesAsync(),
                    AvailableClubs = await boardGameSessionsService.GetAllClubsAsync()
                };

                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Manage));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddGameSessionViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.AvailableBoardGames = await boardGameSessionsService.GetAllBoardGamesAsync();
                    model.AvailableClubs = await boardGameSessionsService.GetAllClubsAsync();
                    return View(model);
                }

                var organizerId = this.GetUserId();
                if (!organizerId.HasValue)
                {
                    return Unauthorized();
                }

                var sessionId = await boardGameSessionsService.AddGameSessionAsync(model, organizerId.Value);
                return RedirectToAction("Manage", "GameSessions", new { area = "", id = sessionId });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Manage));
            }
        }
        
    }
}
