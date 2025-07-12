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
        private readonly ICategoryService categoryService;

        public BoardGameController(IBoardGameService boardGameService, ICategoryService categoryService)
        {
            this.boardGameService = boardGameService;
            this.categoryService = categoryService;
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            try
            {
                AddBoardGameInputModel inputModel = new AddBoardGameInputModel()
                {                    
                    Categories = await this.categoryService.GetCategoriesDropDownDataAsync(),
                };

                return this.View(inputModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Index));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add(AddBoardGameInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(inputModel);
                }

                bool createResult = await this.boardGameService.AddBoardGameAsync(this.GetUserId(), inputModel);

                if (createResult == false)
                {
                    ModelState.AddModelError(string.Empty, "Fatal error occured while adding a boardgame!");

                    return this.View(inputModel);
                }

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Index));
            }
        }

        [AllowAnonymous]
        [HttpGet]
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            try
            {
                Guid? userId = this.GetUserId();

                EditBoardGameInputModel? editInputModel = await this.boardGameService
                    .GetBoardGameForEditingAsync(userId, id);

                if (editInputModel == null)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                editInputModel.Categories = await this.categoryService
                    .GetCategoriesDropDownDataAsync();

                return this.View(editInputModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Index));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Edit(EditBoardGameInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(inputModel);
                }

                bool editResult = await this.boardGameService
                    .PersistUpdatedGameBoardAsync(this.GetUserId(), inputModel);

                if (editResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, "Fatal error occured while updating the boardgame!");

                    return this.View(inputModel);
                }

                return this.RedirectToAction(nameof(Details), new { id = inputModel.Id });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Index));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                Guid? userId = this.GetUserId();

                DeleteBoardGameInputModel deleteInputModel = await this.boardGameService
                    .GetBoardGameForDeletingAsync(userId, id);

                if (deleteInputModel == null)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                return this.View(deleteInputModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Index));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(DeleteBoardGameInputModel inputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Please do not modify the page!");
                    return this.View(inputModel);
                }

                bool deleteResult = await this.boardGameService
                    .SoftDeleteBoardGameAsync(this.GetUserId(), inputModel);

                if (deleteResult == false)
                {
                    this.ModelState.AddModelError(string.Empty, "Fatal error occured while deleting the boardgame!");

                    return this.View(inputModel);
                }

                return this.RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Index));
            }
        }
    }
}
