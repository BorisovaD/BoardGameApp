namespace BoardGameApp.Services.Core.Admin.Interfaces
{
    using BoardGameApp.Web.ViewModels.Admin.BoardGameManagement;
    using BoardGameApp.Web.ViewModels.BoardGame;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBoardGameService
    {
        Task<IEnumerable<AllBoardGamesIndexViewModel>> GetAllBoardGamesAsync(Guid? userId);

        Task<bool> AddBoardGameAsync(Guid? userId, AddBoardGameInputModel inputModel);

        Task<BoardGameDetailsViewModel> GetBoardGameDetailsAsync(Guid? id, Guid? userId);

        Task<EditBoardGameInputModel?> GetBoardGameForEditingAsync(Guid? userId, Guid? boardGameId);

        Task<bool> PersistUpdatedGameBoardAsync(Guid? userId, EditBoardGameInputModel inputModel);

        Task<DeleteBoardGameInputModel?> GetBoardGameForDeletingAsync(Guid? userId, Guid? boardGameId);

        public Task<bool> SoftDeleteBoardGameAsync(Guid? userId, DeleteBoardGameInputModel inputModel);

        Task<IEnumerable<FavoritesBoardGameViewModel>?> GetUserFavoritesBoardGameAsync(Guid? userId);

        Task<bool> AddBoardGameToUserFavoritesListAsync(Guid? userId, Guid boardGameId);

        Task<bool> RemoveBoardGameFromUserFavoritesListAsync(Guid? userId, Guid boardGameId);
    }
}
