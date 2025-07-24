namespace BoardGameApp.Services.Core.Contracts
{
    using BoardGameApp.Web.ViewModels.Club;
    using BoardGameApp.Web.ViewModels.GameSession;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGameSessionService
    {
        Task<IEnumerable<GameSessionsViewModel>> GetAllActiveGameSessions();
    }
}
