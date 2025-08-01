namespace BoardGameApp.Services.Core.Manager
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Manager.Interfaces;
    using BoardGameApp.Web.ViewModels.Manager.GameRanking;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GameRankingService : IGameRankingService
    {
        private readonly IRepository<GameRanking> gameRankingRepository;
        public GameRankingService(IRepository<GameRanking> gameRankingRepository)
        {
            this.gameRankingRepository = gameRankingRepository;
        }
        public async Task<IEnumerable<ManageGameRankingViewModel>> GetAllGameRankingsAsync()
        {
            return await this.gameRankingRepository
                .All()
                .Include(gr => gr.User)
                .Include(gr => gr.BoardGame)
                .Select(gr => new ManageGameRankingViewModel
                {
                    Id = gr.Id,
                    Wins = gr.Wins,
                    Losses = gr.Losses,
                    Draws = gr.Draws,
                    LastUpdated = gr.LastUpdated,
                    Username = gr.User.UserName!,
                    BoardGameTitle = gr.BoardGame.Title
                })
                .ToListAsync();
        }

        public async Task UpdateAsync(GameRankingBaseModel model)
        {
            GameRanking? ranking = await gameRankingRepository
                .FirstOrDefaultAsync(r => r.Id == model.Id);

            if (ranking == null)
            {
                throw new ArgumentException("Invalid ranking ID.");
            }

            ranking.Wins = model.Wins;
            ranking.Losses = model.Losses;
            ranking.Draws = model.Draws;

            await gameRankingRepository.SaveChangesAsync();
        }
    }
}
