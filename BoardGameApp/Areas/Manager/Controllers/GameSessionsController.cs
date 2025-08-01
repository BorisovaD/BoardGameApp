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
                    organizerId.Value);
                                
                return RedirectToAction("Index", "GameSession", new { id = sessionId, area = "" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while saving session: " + ex.Message);
                return StatusCode(500, "An error occurred while saving the game session.");
            }
        }
    }
}
