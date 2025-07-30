namespace BoardGameApp.Services.Core.Admin
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Admin.Interfaces;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.Admin.BoardGameManagement;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BoardGameManagementService : BoardGameService, IBoardGameManagementService
    {
        private readonly IRepository<BoardGame> boardGameRepository;
        public BoardGameManagementService(IRepository<BoardGame> boardGameRepository) 
            : base(boardGameRepository)
        {
            this.boardGameRepository = boardGameRepository;
        }
        public async Task<IEnumerable<BoardGameManagementViewModel>> GetBoardGamesManagementInfoAsync()
        {
            List<BoardGameManagementViewModel> games = await boardGameRepository
                .All()
                .Include(g => g.BoardGameCategories)
                .ThenInclude(c => c.Category)
                .OrderBy(g => g.Title)
                .Select(g => new BoardGameManagementViewModel
                {
                    Id = g.Id,
                    Title = g.Title,
                    Duration = g.Duration,
                    MinPlayers = g.MinPlayers,
                    MaxPlayers = g.MaxPlayers,
                    IsDeleted = g.IsDeleted,
                    Categories = g.BoardGameCategories
                        .Select(bgc => bgc.Category.Name)
                        .ToList()
                })
                .ToListAsync();

            return games;
        }
    }
}
