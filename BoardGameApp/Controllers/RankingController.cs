namespace BoardGameApp.Controllers
{
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.Ranking;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [AllowAnonymous]
    public class RankingController : BaseController
    {
        private readonly IRankingService rankingService;

        public RankingController(IRankingService rankingService)
        {
            this.rankingService = rankingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            List<RankingViewModel> rankings = (List<RankingViewModel>)await rankingService.GetAllRankingsAsync();

            
            var groupedRankings = rankings
                .GroupBy(r => r.BoardGameTitle)
                .ToDictionary(g => g.Key, g => g.ToList());

            return View(groupedRankings);
        }
    }
}
