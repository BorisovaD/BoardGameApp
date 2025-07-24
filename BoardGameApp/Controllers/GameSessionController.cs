namespace BoardGameApp.Controllers
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Services.Core;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.GameSession;
    using Microsoft.AspNetCore.Mvc;

    public class GameSessionController : BaseController
    {
        private readonly IGameSessionService gameSessionService;
        public GameSessionController(IGameSessionService gameSessionService)
        {
            this.gameSessionService = gameSessionService;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<GameSessionsViewModel> activeGameSessions = await gameSessionService.GetAllActiveGameSessions();
            return View(activeGameSessions);
        }
    }
}
