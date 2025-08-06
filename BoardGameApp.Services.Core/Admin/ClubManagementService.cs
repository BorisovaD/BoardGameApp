namespace BoardGameApp.Services.Core.Admin
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository;
    using BoardGameApp.Data.Repository.Interfaces;
    using BoardGameApp.Services.Core.Admin.Interfaces;
    using BoardGameApp.Web.ViewModels.Admin.ClubManagement;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ClubManagementService : ClubService, IClubManagementService
    {
        private readonly IRepository<Manager> managerRepository;
        private readonly UserManager<BoardgameUser> userManager;
        private readonly IRepository<City> cityRepository;

        public ClubManagementService(IRepository<GameSession> gameSessionRepository,IRepository<Club> baseRepository, IClubRepository clubRepository,
        IRepository<Manager> managerRepository, UserManager<BoardgameUser> userManager, IRepository<City> cityRepository) : base(baseRepository, clubRepository, gameSessionRepository)
        {
            this.managerRepository = managerRepository;
            this.userManager = userManager;
            this.cityRepository = cityRepository;
        }

        public async Task<bool> AddClubAsync(ClubManagementCreateInputModel? inputModel)
        {
            bool result = false;

            if (inputModel == null)
                return false;

            var city =  this.cityRepository.All()
                .FirstOrDefault(c => c.Name == inputModel.CityName);

            if (city == null)
            {
                city = new City
                {
                    Name = inputModel.CityName
                };

                await this.cityRepository.AddAsync(city);
                await this.cityRepository.SaveChangesAsync(); 
            }

            var newClub = new Club
            {
                Name = inputModel.ClubName,
                Address = inputModel.Address,
                City = city,
                CityId = city.Id,
                IsDeleted = false
            };

            await this.baseRepository.AddAsync(newClub);
            await this.baseRepository.SaveChangesAsync(); 

            result = true;

            return result;
        }

        public async Task<bool> EditClubAsync(ClubManagementEditInputModel? inputModel)
        {
            bool result = false;

            if (inputModel == null)
                return false;

            Club? clubToEdit = this.baseRepository.All()                
                .FirstOrDefault(c => c.Id == inputModel.Id);

            if (clubToEdit == null)
                return false;

            City? city = this.cityRepository.All()
                .FirstOrDefault(c => c.Name == inputModel.CityName);

            if (city == null)
            {
                city = new City 
                { 
                    Name = inputModel.CityName 
                };

                await this.cityRepository.AddAsync(city);
            }

            clubToEdit.Name = inputModel.ClubName;
            clubToEdit.Address = inputModel.Address;
            clubToEdit.City = city;

            await this.baseRepository.SaveChangesAsync(); 

            result = true;

            return result;
        }

        public async Task<ClubManagementEditInputModel?> GetClubForEditingAsync(Guid id)
        {
            var clubToEdit = this.baseRepository
                .All()
                .SingleOrDefault(c => c.Id == id);

            if (clubToEdit == null)
            {
                return null;
            }

            return new ClubManagementEditInputModel()
            {
                Id = clubToEdit.Id,
                ClubName = clubToEdit.Name,
                Address = clubToEdit.Address,
                CityName = clubToEdit.City?.Name ?? "Unknown"
            };
        }

        public async Task<IEnumerable<ClubManagementIndexViewModel>> GetClubManagementInfoAsync()
        {
            IEnumerable<ClubManagementIndexViewModel> allClubs = await baseRepository
            .All()
            .Select(c => new ClubManagementIndexViewModel()
            {
                Id = c.Id,
                ClubName = c.Name,
                CityName = c.City.Name,
                IsDeleted = c.IsDeleted,
                ManagerName = c.GameSessions
                    .Select(gs => gs.Organizer.UserName)
                    .FirstOrDefault()
            })
            .ToArrayAsync();

            return allClubs;
        }

        public async Task<(bool Success, bool IsNowDeleted)> ToggleClubDeletionAsync(Guid id)
        {
            Club? clubToToggle = this.baseRepository
                .All()
                .FirstOrDefault(c => c.Id == id);

            if (clubToToggle == null)
            {
                return (false, false);
            }

            if (clubToToggle.IsDeleted)
            {
                await this.baseRepository.ReturnExisting(clubToToggle);
            }
            else
            {
                await this.baseRepository.SoftDeleteAsync(clubToToggle);
            }

            await this.baseRepository.SaveChangesAsync();

            return (true, clubToToggle.IsDeleted);
        }
    }
}
