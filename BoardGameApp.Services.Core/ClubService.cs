namespace BoardGameApp.Services.Core
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.Club;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ClubService : IClubService
    {
        protected readonly IRepository<Club> baseRepository;
        protected readonly IClubRepository clubRepository;
        protected readonly IRepository<GameSession> gameSessionrepository;

        public ClubService(IRepository<Club> baseRepository, IClubRepository clubRepository, IRepository<GameSession> gameSessionrepository)
        {
            this.baseRepository = baseRepository;
            this.clubRepository = clubRepository;
            this.gameSessionrepository = gameSessionrepository;
        }

        public async Task<IEnumerable<ClubMapViewModel>> GetAllActiveClubs()
        {
            IEnumerable<ClubMapViewModel> clubs = await clubRepository.All()  
                .Where(c => !c.IsDeleted)
                .Include(c => c.City)              
                .Select(c => new ClubMapViewModel
                {
                    ClubId = c.Id,
                    ClubName = c.Name,
                    CityId = c.City.Id,
                    CityName = c.City.Name
                })
                .ToArrayAsync();

            return clubs;
        }

        public async Task<ClubDetailsViewModel?> GetClubDetailsAsync(Guid id)
        {
            Club? club = await clubRepository.GetDetailsByIdAsync(id);

            if (club == null)
            {
                return null;
            }

            var ActiveGameSessionId = gameSessionrepository
                .All()
                .Where(gs => !gs.IsDeleted && gs.StartTime > DateTime.Now)
                .OrderBy(gs => gs.StartTime)
                .Select(gs => gs.Id)
                .FirstOrDefault();

            return new ClubDetailsViewModel
            {
                Id = club.Id,
                Name = club.Name,
                Address = club.Address,
                CityName = club.City.Name,
                BoardGames = club.ClubBoardGames.Select(cb => new BoardGameInClubViewModel
                {
                    Id = cb.BoardGame.Id,
                    Title = cb.BoardGame.Title,
                    ImageUrl = cb.BoardGame.ImageUrl,
                    ActiveGameSessionId = ActiveGameSessionId
                })
                .ToArray()
            };
        }
    }
}
