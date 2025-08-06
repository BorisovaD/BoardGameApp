namespace BoardGameApp.Services.Core
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.Ranking;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RankingService : IRankingService
    {
        private readonly IRepository<GameRanking> gameRankingRepository;

        public RankingService(IRepository<GameRanking> gameRankingRepository)
        {
            this.gameRankingRepository = gameRankingRepository;
        }
        public async Task<IEnumerable<RankingViewModel>> GetAllRankingsAsync()
        {
            return await this.gameRankingRepository
            .All()
            .Include(gr => gr.User)
            .Include(gr => gr.BoardGame)
            .Select(gr => new RankingViewModel
            {
                Id = gr.Id,
                Username = gr.User.UserName!,
                BoardGameTitle = gr.BoardGame.Title,
                Wins = gr.Wins,
                Losses = gr.Losses,
                Draws = gr.Draws,
            })
            .ToListAsync();
        }
    }    
}
