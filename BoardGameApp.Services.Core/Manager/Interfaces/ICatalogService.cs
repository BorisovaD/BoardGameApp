namespace BoardGameApp.Services.Core.Manager.Interfaces
{
    using BoardGameApp.Web.ViewModels.BoardGame;
    using BoardGameApp.Web.ViewModels.Manager.ClubBoardGames;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICatalogService
    {
        Task<IEnumerable<ClubBoardGamesCatalogViewModel>> GetAllBoardGamesAsync(Guid clubId);

        public Task<Guid?> GetClubIdByManagerIdAsync(Guid? managerId);
    }
}
