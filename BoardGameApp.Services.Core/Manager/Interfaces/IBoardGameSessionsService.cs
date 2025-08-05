namespace BoardGameApp.Services.Core.Manager.Interfaces
{
    using BoardGameApp.Web.ViewModels.Manager.GameSessions;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBoardGameSessionsService
    {
        Task<IEnumerable<ManageGameSessionViewModel>> GetManageViewModelAsync(Guid clubId);

        Task<Guid> SaveGameSessionAsync(Guid boardGameId, DateTime startTime, DateTime endTime, Guid organizerId, bool isActive);

        Task<IEnumerable<SelectListItem>> GetAllBoardGamesAsync();

        Task<IEnumerable<SelectListItem>> GetAllClubsAsync();

        Task<Guid> AddGameSessionAsync(AddGameSessionViewModel model, Guid organizerId);

        Task<bool> ArchiveAsync(Guid boardGameId);
    }
}
