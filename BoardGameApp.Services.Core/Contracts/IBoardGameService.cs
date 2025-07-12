namespace BoardGameApp.Services.Core.Contracts
{
    using BoardGameApp.Web.ViewModels.BoardGame;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBoardGameService
    {
        Task<IEnumerable<AllBoardGamesIndexViewModel>> GetAllBoardGamesAsync(Guid? userId);
    }
}
