namespace BoardGameApp.Controllers
{
    using BoardGameApp.Data;
    using BoardGameApp.Services.Core.Admin.Interfaces;
    using BoardGameApp.Web.ViewModels.Admin.BoardGameManagement;
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id)
        {
            try
            {
                Guid? userId = this.GetUserId();
                BoardGameDetailsViewModel boardGameDetails = await this.boardGameService.GetBoardGameDetailsAsync(id, userId);
                if (boardGameDetails == null)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                return this.View(boardGameDetails);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return this.RedirectToAction(nameof(Index), "Home");
            }
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Favorites()
        {
            try
            {
                Guid? userId = this.GetUserId();

                IEnumerable<FavoritesBoardGameViewModel>? favRecipes = await this.boardGameService.GetUserFavoritesBoardGameAsync(userId);

                if (favRecipes == null)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                return this.View(favRecipes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToFavorites(Guid? id)
        {
            try
            {
                Guid? userId = this.GetUserId();

                if (id == null)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                bool favAddResult = await this.boardGameService
                    .AddBoardGameToUserFavoritesListAsync(userId, id.Value);
                if (favAddResult == false)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromFavorites(Guid? id)
        {
            try
            {
                Guid? userId = this.GetUserId();

                if (id == null)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                bool favAddResult = await this.boardGameService
                    .RemoveBoardGameFromUserFavoritesListAsync(userId, id.Value);
                if (favAddResult == false)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                return this.RedirectToAction(nameof(Favorites));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Index));
            }
        }
    }
}
