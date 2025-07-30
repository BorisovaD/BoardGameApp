namespace BoardGameApp.Areas.Admin.Controllers
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Admin.Interfaces;
    using BoardGameApp.Web.ViewModels.Admin.BoardGameManagement;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
    using NuGet.Protocol.Core.Types;

    public class BoardGameManagementController : BaseAdminController
    {
        private readonly IRepository<BoardGame> boardGameRepository;
        private readonly IBoardGameManagementService boardGameManagementService;
        private readonly IBoardGameService boardGameService;
        private readonly ICategoryService categoryService;
        public BoardGameManagementController(IRepository<BoardGame> boardGameRepository,IBoardGameManagementService boardGameManagementService, IBoardGameService boardGameService, ICategoryService categoryService)
        {
            this.boardGameRepository = boardGameRepository;
            this.boardGameManagementService = boardGameManagementService;
            this.boardGameService = boardGameService;
            this.categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            IEnumerable<BoardGameManagementViewModel> allBoardGames = await this.boardGameManagementService
                .GetBoardGamesManagementInfoAsync();

            return View(allBoardGames);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
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
                return this.RedirectToAction(nameof(Manage));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddBoardGameInputModel inputModel)
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

                return this.RedirectToAction(nameof(Manage));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Manage));
            }
        }

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
                    return this.RedirectToAction(nameof(Manage));
                }

                editInputModel.Categories = await this.categoryService
                    .GetCategoriesDropDownDataAsync();

                return this.View(editInputModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Manage));
            }
        }

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

                return this.RedirectToAction(nameof(Manage), new { id = inputModel.Id });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Manage));
            }
        }

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
                    return this.RedirectToAction(nameof(Manage));
                }

                return this.View(deleteInputModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Manage));
            }
        }
             
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

                return this.RedirectToAction(nameof(Manage));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Manage));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Restore(Guid id)
        {
            BoardGame? game = await boardGameRepository.GetByIdAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            await boardGameRepository.ReturnExisting(game);
            await boardGameRepository.SaveChangesAsync(); 

            return RedirectToAction(nameof(Manage));
        }
    }
}
