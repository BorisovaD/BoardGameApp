namespace BoardGameApp.Services.Core.Manager.Interfaces
{
    using BoardGameApp.Web.ViewModels.Manager.GameSessions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBoardGameSessionsService
    {
        Task<IEnumerable<ManageGameSessionViewModel>> GetManageViewModelAsync(Guid clubId);

        Task<bool> SaveGameSessionTimeAsync(Guid gameId, int startHour, int endHour);
    }
}
