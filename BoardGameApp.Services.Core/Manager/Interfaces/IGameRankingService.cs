namespace BoardGameApp.Services.Core.Manager.Interfaces
{
    using BoardGameApp.Web.ViewModels.Manager.GameRanking;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGameRankingService
    {
        Task<IEnumerable<ManageGameRankingViewModel>> GetAllGameRankingsAsync();

        Task UpdateAsync(GameRankingBaseModel model);
    }
}
