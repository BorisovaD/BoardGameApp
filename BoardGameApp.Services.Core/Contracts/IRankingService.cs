namespace BoardGameApp.Services.Core.Contracts
{
    using BoardGameApp.Web.ViewModels.Ranking;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IRankingService
    {
        Task<IEnumerable<RankingViewModel>> GetAllRankingsAsync();
    }
}
