namespace BoardGameApp.Services.Core
{
    using BoardGameApp.Data;
    using BoardGameApp.Data.Models;
    using BoardGameApp.Services.Core.Contracts;
    using BoardGameApp.Web.ViewModels.BoardGame;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BoardGameService : IBoardGameService
    {
        private readonly BoardGameAppDbContext dbContext;
        private readonly UserManager<IdentityUser> userManager;
        public BoardGameService(BoardGameAppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public async Task<bool> AddBoardGameAsync(Guid? userId, AddBoardGameInputModel inputModel)
        {
            bool opResult = false;

            IdentityUser? user = await this.userManager.FindByIdAsync(userId.ToString()!);

            if ((user != null) && inputModel.SelectedCategoryIds.Any())
            {
                BoardGame newBoardGame = new BoardGame()
                {
                    Title = inputModel.Title,
                    Description = inputModel.Description,
                    ImageUrl = inputModel.ImageUrl,
                    RulesUrl = inputModel.RulesUrl,
                    MinPlayers = inputModel.MinPlayers,
                    MaxPlayers = inputModel.MaxPlayers,
                    Duration = inputModel.Duration
                };
                                
                foreach (var categoryId in inputModel.SelectedCategoryIds)
                {
                    newBoardGame.BoardGameCategories.Add(new BoardGameCategory
                    {
                        CategoryId = categoryId,
                        BoardGameId = newBoardGame.Id
                    });
                }

                await this.dbContext.BoardGames.AddAsync(newBoardGame);
                await this.dbContext.SaveChangesAsync();

                opResult = true;
            }

            return opResult;
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

        public async Task<BoardGameDetailsViewModel> GetBoardGameDetailsAsync(Guid? id, Guid? userId)
        {
            BoardGameDetailsViewModel? detailsVm = null;

            if (id.HasValue)
            {
                BoardGame? boardGameModel = await this.dbContext
                    .BoardGames
                    .Where(g => g.IsDeleted == false)
                    .Include(g => g.BoardGameCategories)
                    .ThenInclude(bgc => bgc.Category)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(g => g.Id == id);

                if (boardGameModel != null)
                {
                    detailsVm = new BoardGameDetailsViewModel()
                    {
                        Id = boardGameModel.Id.ToString(),
                        Title = boardGameModel.Title,
                        Description = boardGameModel.Description,
                        ImageUrl = boardGameModel.ImageUrl,
                        RulesUrl = boardGameModel.RulesUrl,
                        MinPlayers = boardGameModel.MinPlayers.ToString(),
                        MaxPlayers = boardGameModel.MaxPlayers.ToString(),
                        Duration = boardGameModel.Duration.ToString(),
                        IsSaved = userId != null ?
                        boardGameModel.FavoritedByUsers.Any(f => f.UserId == userId) : false,
                        Categories = boardGameModel.BoardGameCategories
                                         .Select(bc => bc.Category.Name)
                                         .ToList()
                    };
                }
            }

            return detailsVm!;
        }

        public async Task<DeleteBoardGameInputModel?> GetBoardGameForDeletingAsync(Guid? userId, Guid? boardGameId)
        {
            DeleteBoardGameInputModel? deleteModel = null;

            if (boardGameId != null)
            {
                BoardGame? deleteBoardGameModel = await this.dbContext
                    .BoardGames
                    .Where(g => g.IsDeleted == false)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(g => g.Id == boardGameId);

                if (deleteBoardGameModel != null)
                {
                    deleteModel = new DeleteBoardGameInputModel()
                    {
                        Id = deleteBoardGameModel.Id,
                        Title = deleteBoardGameModel.Title,
                        ImageUrl = deleteBoardGameModel.ImageUrl,
                    };
                }
            }

            return deleteModel;
        }

        public async Task<EditBoardGameInputModel?> GetBoardGameForEditingAsync(Guid? userId, Guid? boardGameId)
        {
            EditBoardGameInputModel? editModel = null;

            if (boardGameId != null)
            {
                BoardGame? editBoardGameModel = await this.dbContext
                    .BoardGames
                    .Where(g => g.IsDeleted == false)
                    .Include(g => g.BoardGameCategories)
                    .ThenInclude(bc => bc.Category)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(g => g.Id == boardGameId);

                if (editBoardGameModel != null)
                {
                    editModel = new EditBoardGameInputModel()
                    {
                        Id = editBoardGameModel.Id,
                        Title = editBoardGameModel.Title,
                        Description = editBoardGameModel.Description,
                        ImageUrl = editBoardGameModel.ImageUrl,
                        RulesUrl = editBoardGameModel.RulesUrl,
                        MinPlayers = editBoardGameModel.MinPlayers,
                        MaxPlayers = editBoardGameModel.MaxPlayers,
                        Duration = editBoardGameModel.Duration,
                        SelectedCategoryIds = editBoardGameModel.BoardGameCategories
                                .Select(c => c.CategoryId)
                                .ToList()
                    };
                }
            }

            return editModel;
        }

        public async Task<bool> PersistUpdatedGameBoardAsync(Guid? userId, EditBoardGameInputModel inputModel)
        {
            bool opResult = false;

            IdentityUser? user = await this.userManager.FindByIdAsync(userId.ToString()!);

            BoardGame? updatedBoardGame = await this.dbContext
                .BoardGames
                .Include(bg => bg.BoardGameCategories)
                .FirstOrDefaultAsync(bg => bg.Id == inputModel.Id);

            if ((user != null) && (updatedBoardGame != null))
            {
                updatedBoardGame.Title = inputModel.Title;
                updatedBoardGame.Description = inputModel.Description;
                updatedBoardGame.ImageUrl = inputModel.ImageUrl;
                updatedBoardGame.RulesUrl = inputModel.RulesUrl;
                updatedBoardGame.MinPlayers = inputModel.MinPlayers;
                updatedBoardGame.MaxPlayers = inputModel.MaxPlayers;
                updatedBoardGame.Duration = inputModel.Duration;

                updatedBoardGame.BoardGameCategories.Clear();

                foreach (var categoryId in inputModel.SelectedCategoryIds)
                {
                    updatedBoardGame.BoardGameCategories.Add(new BoardGameCategory
                    {
                        BoardGameId = updatedBoardGame.Id,
                        CategoryId = categoryId
                    });
                }

                await this.dbContext.SaveChangesAsync();

                opResult = true;
            }

            return opResult;
        }

        public async Task<bool> SoftDeleteBoardGameAsync(Guid? userId, DeleteBoardGameInputModel inputModel)
        {
            bool opResult = false;

            IdentityUser? user = await this.userManager.FindByIdAsync(userId.ToString()!);

            BoardGame? deletedBoardGame = await this.dbContext
                .BoardGames
                .FindAsync(inputModel.Id);


            if ((user != null) && (deletedBoardGame != null))
            {
                deletedBoardGame.IsDeleted = true;

                await this.dbContext.SaveChangesAsync();

                opResult = true;
            }

            return opResult;
        }
    }
}
