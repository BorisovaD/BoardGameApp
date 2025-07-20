namespace BoardGameApp.Services.Core
{
    using BoardGameApp.Data;
    using BoardGameApp.Data.Models;
    using BoardGameApp.Data.Repository.Interfaces;
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
        private readonly IRepository<BoardGame> boardGameRepository;
        private readonly IRepository<BoardgameUserFavorite> favoritesRepository;
        private readonly UserManager<BoardgameUser> userManager;
        public BoardGameService(IRepository<BoardGame> boardGameRepository, 
                                IRepository<BoardgameUserFavorite> favoritesRepository,
                                UserManager<BoardgameUser> userManager)
        {
            this.boardGameRepository = boardGameRepository;
            this.favoritesRepository = favoritesRepository;
            this.userManager = userManager;
        }

        public async Task<bool> AddBoardGameAsync(Guid? userId, AddBoardGameInputModel inputModel)
        {
            bool opResult = false;

            if (userId.HasValue)
            {
                BoardgameUser? user = await this.userManager.FindByIdAsync(userId.Value.ToString());


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
                        });
                    }

                    await this.boardGameRepository.AddAsync(newBoardGame);
                    await this.boardGameRepository.SaveChangesAsync();

                    opResult = true;
                }
            }

            return opResult;
        }

        public async Task<bool> AddBoardGameToUserFavoritesListAsync(Guid? userId, Guid boardGameId)
        {
            bool opResult = false;

            if (userId == null)
            {
                return false;
            }

            BoardGame? favBoardGame = await this.boardGameRepository.GetByIdAsync(boardGameId);

            if (favBoardGame != null)
            {     
                BoardgameUserFavorite? userFavBoardGame = await this.favoritesRepository
                    .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.BoardGameId == boardGameId);

                if (userFavBoardGame != null)
                {
                    await this.favoritesRepository.ReturnExisting(userFavBoardGame);
                    await this.favoritesRepository.SaveChangesAsync();

                }
                else
                {
                    
                    var newFavorite = new BoardgameUserFavorite
                    {
                        UserId = userId.Value,
                        BoardGameId = boardGameId
                    };

                    await this.favoritesRepository.AddAsync(newFavorite);
                    await this.favoritesRepository.SaveChangesAsync();
                }

                opResult = true;                
            }

            return opResult;
        }

        public async Task<IEnumerable<AllBoardGamesIndexViewModel>> GetAllBoardGamesAsync(Guid? userId)
        {
            AllBoardGamesIndexViewModel[]? allGames = await boardGameRepository
                .All()                
                .Where(g => g.IsDeleted == false)
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
                    IsSaved = userId != null && g.FavoritedByUsers.Any(f => f.UserId == userId)
                })
                .ToArrayAsync();

            return allGames;
        }

        public async Task<BoardGameDetailsViewModel> GetBoardGameDetailsAsync(Guid? id, Guid? userId)
        {
            BoardGameDetailsViewModel? detailsVm = null;

            if (id.HasValue)
            {
                BoardGame? boardGameModel = await boardGameRepository
                    .All()
                    .Where(g => g.IsDeleted == false)
                    .Include(g => g.BoardGameCategories)
                    .ThenInclude(bgc => bgc.Category)
                    .Include(g => g.FavoritedByUsers)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(g => g.Id == id.Value);

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
                        IsSaved = userId != null && 
                        boardGameModel.FavoritedByUsers.Any(f => f.UserId == userId),
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
                BoardGame? deleteBoardGameModel = await boardGameRepository
                    .All()
                    .Where(g => g.IsDeleted == false)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(g => g.Id == boardGameId.Value);

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
                BoardGame? editBoardGameModel = await boardGameRepository 
                    .All()                    
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

        public async Task<IEnumerable<FavoritesBoardGameViewModel>?> GetUserFavoritesBoardGameAsync(Guid? userId)
        {  
            if (userId == null)
            {
                return Enumerable.Empty<FavoritesBoardGameViewModel>();
            }

            IEnumerable<FavoritesBoardGameViewModel> favBoardGames = await favoritesRepository
                .All()
                .Include(uf => uf.BoardGame)
                .ThenInclude(bc => bc.BoardGameCategories)
                .ThenInclude(c => c.Category)
                .Where(bc => bc.UserId == userId && !bc.BoardGame.IsDeleted)
                .Select(f => new FavoritesBoardGameViewModel()
                {
                    Id = f.BoardGameId,
                    Title = f.BoardGame.Title,
                    Description = f.BoardGame.Description,
                    ImageUrl = f.BoardGame.ImageUrl,
                    RulesUrl = f.BoardGame.RulesUrl,
                    MinPlayers = f.BoardGame.MinPlayers.ToString(),
                    MaxPlayers = f.BoardGame.MaxPlayers.ToString(),
                    Duration = f.BoardGame.Duration.ToString(),
                    Categories = f.BoardGame.BoardGameCategories
                                     .Select(bc => bc.Category.Name)
                                     .ToList()
                })
                .ToArrayAsync();

            return favBoardGames;
        }

        public async Task<bool> PersistUpdatedGameBoardAsync(Guid? userId, EditBoardGameInputModel inputModel)
        {
            bool opResult = false;

            if (userId.HasValue)
            {
                BoardgameUser? user = await this.userManager.FindByIdAsync(userId.Value.ToString());


                BoardGame? updatedBoardGame = await boardGameRepository
                    .All()
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

                    await boardGameRepository.SaveChangesAsync();

                    opResult = true;
                }
            }

            return opResult;
        }

        public async Task<bool> RemoveBoardGameFromUserFavoritesListAsync(Guid? userId, Guid boardGameId)
        {
            bool opResult = false;

            if (userId == null)
            {
                return false;
            }


            BoardgameUserFavorite? userFavBoardGame = await this.favoritesRepository
                    .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.BoardGameId == boardGameId);

            if (userFavBoardGame != null)
            {
                this.favoritesRepository.Delete(userFavBoardGame);
                await this.favoritesRepository.SaveChangesAsync();

                opResult = true;
            }
            

            return opResult;
        }

        public async Task<bool> SoftDeleteBoardGameAsync(Guid? userId, DeleteBoardGameInputModel inputModel)
        {
            bool opResult = false;

            if (userId.HasValue)
            {
                BoardgameUser? user = await this.userManager.FindByIdAsync(userId.Value.ToString());


                BoardGame? deletedBoardGame = await boardGameRepository.GetByIdAsync(inputModel.Id);


                if ((user != null) && (deletedBoardGame != null))
                {
                    deletedBoardGame.IsDeleted = true;

                    await boardGameRepository.SaveChangesAsync();

                    opResult = true;
                }
            }
            return opResult;
        }
    }
}
