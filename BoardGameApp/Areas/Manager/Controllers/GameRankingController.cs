namespace BoardGameApp.Areas.Manager.Controllers
{
    using BoardGameApp.Controllers;
    using BoardGameApp.Services.Core.Manager;
    using BoardGameApp.Services.Core.Manager.Interfaces;
    using BoardGameApp.Web.ViewModels.Manager.GameRanking;
    using Microsoft.AspNetCore.Mvc;

    [Area("Manager")]
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
            var model = await gameRankingService.GetAllGameRankingsAsync();
           
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(GameRankingBaseModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Manage));
            }

            await gameRankingService.UpdateAsync(model);

            return RedirectToAction(nameof(Manage));
        }
    }
}
