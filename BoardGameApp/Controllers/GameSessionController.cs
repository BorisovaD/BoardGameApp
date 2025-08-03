namespace BoardGameApp.Controllers
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Services.Core;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.GameSession;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class GameSessionController : BaseController
    {
        private readonly IGameSessionService gameSessionService;
        public GameSessionController(IGameSessionService gameSessionService)
        {
            this.gameSessionService = gameSessionService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {            
            try
            {
                IEnumerable<GameSessionsViewModel> activeGameSessions = await gameSessionService.GetAllActiveGameSessions();

                return View(activeGameSessions);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }
        }
    }
}
