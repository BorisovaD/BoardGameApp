namespace BoardGameApp.Services.Core.Admin.Interfaces
{
    using BoardGameApp.Web.ViewModels.Admin.BoardGameManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBoardGameManagementService
    {
        Task<IEnumerable<BoardGameManagementViewModel>> GetBoardGamesManagementInfoAsync();
    }
}
