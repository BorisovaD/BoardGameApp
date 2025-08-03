namespace BoardGameApp.Areas.Manager.Controllers
{
    using BoardGameApp.Controllers;
    using BoardGameApp.Services.Core.Manager;
    using BoardGameApp.Services.Core.Manager.Interfaces;
    using BoardGameApp.Web.ViewModels.Manager.GameRanking;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using static BoardGameApp.GCommon.ApplicationConstants;

    [Area("Manager")]
    [Authorize(Roles = RoleManager)]
    public class GameRankingController : BaseController
    {
        private readonly IGameRankingService gameRankingService;

        public GameRankingController(IGameRankingService gameRankingService)
        {
            this.gameRankingService = gameRankingService;
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            try
            {
                var model = await gameRankingService.GetAllGameRankingsAsync();

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
        public async Task<IActionResult> Update(GameRankingBaseModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Manage));
                }

                await gameRankingService.UpdateAsync(model);

                return RedirectToAction(nameof(Manage));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return this.RedirectToAction(nameof(Manage));
            }
        }
    }
}
