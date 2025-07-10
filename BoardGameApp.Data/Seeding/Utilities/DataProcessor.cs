namespace BoardGameApp.Data.Seeding.Utilities
{
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Seeding.Dtos;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class DataProcessor
    {
        public static void ImportBoardGames(BoardGameAppDbContext dbContext)
        {
            if (dbContext.BoardGames.Any())
            {
                return; 
            }
            var json = File.ReadAllText("../../BoardGameApp/BoardGameApp.Data/Seeding/Files/BoardGames.json");

            var validGames = new List<BoardGame>();

            var boardGameDtos = JsonConvert.DeserializeObject<List<ImportBoardGameDto>>(json);

            foreach (var dto in boardGameDtos!)
            {
                if (!IsValid(dto))
                {
                    continue;
                }

                var game = new BoardGame
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    ImageUrl = dto.ImageUrl,
                    RulesUrl = dto.RulesUrl,
                    MinPlayers = dto.MinPlayers,
                    MaxPlayers = dto.MaxPlayers,
                    Duration = dto.Duration,
                    IsDeleted = dto.IsDeleted
                };

                validGames.Add(game);
            }
            dbContext.BoardGames.AddRange(validGames);
            dbContext.SaveChanges();
        }

        public static void ImportCategories(BoardGameAppDbContext dbContext)
        {
            if (dbContext.Categories.Any())
            {
                return; 
            }

            var json = File.ReadAllText("../../BoardGameApp/BoardGameApp.Data/Seeding/Files/Categories.json");

            var validCategories = new List<Category>();

            var categoryDtos = JsonConvert.DeserializeObject<List<ImportCategoryDto>>(json);

            foreach (var dto in categoryDtos!)
            {
                if (!IsValid(dto))
                {
                    continue;
                }

                var category = new Category
                {
                    Id = dto.Id,
                    Name = dto.Name,                    
                    IsDeleted = dto.IsDeleted
                };

                validCategories.Add(category);
            }
            dbContext.Categories.AddRange(validCategories);
            dbContext.SaveChanges();
        }

        public static void ImportBoardGameCategories(BoardGameAppDbContext dbContext)
        {
            if (dbContext.BoardGameCategories.Any())
            {
                return;
            }

            var json = File.ReadAllText("../../BoardGameApp/BoardGameApp.Data/Seeding/Files/BoardGameCategories.json");

            var validCategories = new List<BoardGameCategory>();

            var categoryDtos = JsonConvert.DeserializeObject<List<ImportBoardGameCategoryDto>>(json);

            foreach (var dto in categoryDtos!)
            {
                if (!IsValid(dto))
                {
                    continue;
                }

                var boardGameCategory = new BoardGameCategory
                {
                    CategoryId = dto.CategoryId,
                    BoardGameId = dto.BoardGameId,
                    IsDeleted = dto.IsDeleted
                };

                validCategories.Add(boardGameCategory);
            }
            dbContext.BoardGameCategories.AddRange(validCategories);
            dbContext.SaveChanges();
        }

        public static void ImportCities(BoardGameAppDbContext dbContext)
        {
            if (dbContext.Cities.Any())
            {
                return;
            }

            var json = File.ReadAllText("../../BoardGameApp/BoardGameApp.Data/Seeding/Files/Cities.json");

            var validCities = new List<City>();

            var cityDtos = JsonConvert.DeserializeObject<List<ImportCityDto>>(json);

            foreach (var dto in cityDtos!)
            {
                if (!IsValid(dto))
                {
                    continue;
                }

                var city = new City
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    IsDeleted = dto.IsDeleted
                };

                validCities.Add(city);
            }
            dbContext.Cities.AddRange(validCities);
            dbContext.SaveChanges();
        }

        public static void ImportClubs(BoardGameAppDbContext dbContext)
        {
            if (dbContext.Clubs.Any())
            {
                return;
            }

            var json = File.ReadAllText("../../BoardGameApp/BoardGameApp.Data/Seeding/Files/Clubs.json");

            var validClubs = new List<Club>();

            var clubDtos = JsonConvert.DeserializeObject<List<ImportClubDto>>(json);

            foreach (var dto in clubDtos!)
            {
                if (!IsValid(dto))
                {
                    continue;
                }

                var club = new Club
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Address = dto.Address,
                    CityId = dto.CityId,
                    IsDeleted = dto.IsDeleted
                };

                validClubs.Add(club);
            }
            dbContext.Clubs.AddRange(validClubs);
            dbContext.SaveChanges();
        }

        public static void ImportClubBoardGames(BoardGameAppDbContext dbContext)
        {
            if (dbContext.ClubBoardGames.Any())
            {
                return;
            }

            var json = File.ReadAllText("../../BoardGameApp/BoardGameApp.Data/Seeding/Files/ClubBoardGames.json");

            var validClubBoardGames = new List<ClubBoardGame>();

            var clubBoardGameDtos = JsonConvert.DeserializeObject<List<ImportClubBoardGameDto>>(json);

            foreach (var dto in clubBoardGameDtos!)
            {
                if (!IsValid(dto))
                {
                    continue;
                }

                var clubBoardGame = new ClubBoardGame
                {
                    ClubId = dto.ClubId,
                    BoardGameId = dto.BoardGameId,
                    IsDeleted = dto.IsDeleted
                };

                validClubBoardGames.Add(clubBoardGame);
            }
            dbContext.ClubBoardGames.AddRange(validClubBoardGames);
            dbContext.SaveChanges();
        }

        private static bool IsValid(object dto)
        {
            var validateContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator
                .TryValidateObject(dto, validateContext, validationResults, true);

            return isValid;
        }
    }
}
