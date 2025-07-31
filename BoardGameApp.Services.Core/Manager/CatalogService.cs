namespace BoardGameApp.Services.Core.Manager
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Manager.Interfaces;
    using BoardGameApp.Web.ViewModels.BoardGame;
    using BoardGameApp.Web.ViewModels.Manager.ClubBoardGames;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CatalogService : ICatalogService
    {
        private readonly IRepository<BoardGame> boardGameRepository;
        private readonly IRepository<ClubBoardGame> clubBoardGameRepository;
        private readonly IRepository<Manager> managerRepository;
        public CatalogService(IRepository<BoardGame> boardGameRepository, IRepository<ClubBoardGame> clubBoardGameRepository, IRepository<Manager> managerRepository)
        {
            this.boardGameRepository = boardGameRepository;
            this.clubBoardGameRepository = clubBoardGameRepository;
            this.managerRepository = managerRepository;
        }
        public async Task<IEnumerable<ClubBoardGamesCatalogViewModel>> GetAllBoardGamesAsync(Guid clubId)
        {
            ClubBoardGamesCatalogViewModel[]? allGames = await boardGameRepository
                .All()                
                .Where(g => g.IsDeleted == false)
                .Include(g => g.ClubBoardGames)
                .Select(g => new ClubBoardGamesCatalogViewModel
                {
                    BoardGameId = g.Id,
                    ClubId = clubId,
                    Title = g.Title,
                    ImageUrl = g.ImageUrl,
                    MinPlayers = g.MinPlayers,
                    MaxPlayers = g.MaxPlayers,
                    Duration = g.Duration,
                    IsInClub = g.ClubBoardGames.Any(cb => cb.ClubId == clubId)
                })
                .ToArrayAsync();

            return allGames;
        }

        public async Task<Guid?> GetClubIdByManagerIdAsync(Guid? managerId)
        {
            return await managerRepository
                .All()
                .Where(m => m.UserId == managerId)
                .Select(m => (Guid?)m.ClubId)
                .FirstOrDefaultAsync();
        }
    }
}
