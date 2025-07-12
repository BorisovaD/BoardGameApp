namespace BoardGameApp.Controllers
{
    using BoardGameApp.Data;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.BoardGame;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class BoardGameController : BaseController
    {
        private readonly IBoardGameService boardGameService;

        public BoardGameController(IBoardGameService boardGameService)
        {
            this.boardGameService = boardGameService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                Guid? userId = this.GetUserId();
                IEnumerable<AllBoardGamesIndexViewModel> allBoardGames = await
                    this.boardGameService.GetAllBoardGamesAsync(userId);

                return View(allBoardGames);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }
        }
    }
}
