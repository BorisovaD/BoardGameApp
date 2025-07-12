namespace BoardGameApp.Services.Core
{
    using BoardGameApp.Data;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.BoardGame;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BoardGameService : IBoardGameService
    {
        private readonly BoardGameAppDbContext dbContext;
        public BoardGameService(BoardGameAppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<AllBoardGamesIndexViewModel>> GetAllBoardGamesAsync(Guid? userId)
        {
            IEnumerable<AllBoardGamesIndexViewModel> allGames = await dbContext
                .BoardGames
                .Where(g => g.IsDeleted == false)
                .Include(g => g.FavoritedByUsers)
                .Select(g => new AllBoardGamesIndexViewModel
                {
                    Id = g.Id.ToString(),
                    Title = g.Title,
                    Description = g.Description,
                    ImageUrl = g.ImageUrl,
                    RulesUrl = g.RulesUrl,
                    MinPlayers = g.MinPlayers.ToString(),
                    MaxPlayers = g.MaxPlayers.ToString(),
                    Duration = g.Duration.ToString(),                 
                    IsSaved = userId != null ? 
                        g.FavoritedByUsers.Any(f => f.UserId == userId) : false,
                })
                .ToArrayAsync();

            return allGames;
        }
    }
}
