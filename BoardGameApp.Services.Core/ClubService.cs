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
        private readonly IRepository<Club> clubRepository;

        public ClubService(IRepository<Club> clubRepository)
        {
            this.clubRepository = clubRepository;
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
    }
}
